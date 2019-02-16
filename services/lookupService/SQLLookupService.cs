using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration;
using RatingAPI.services.lookupService;
using Newtonsoft.Json.Linq;
using Serilog;


namespace RatingAPI.services.lookupService
{
    public class SQLLookupServiceConfig
    {
        public string connectionString {get; set;}
        public string providerName {get; set;}
    }

    public class SQLLookupService : ILookupService
    {
        private readonly SQLLookupServiceConfig _config;
        public SQLLookupService(IConfiguration configuration)
        {
            this._config = new SQLLookupServiceConfig();
            configuration.Bind("LookupServices:SQLLookupService", this._config);

        }
        public LookupResponse GetFeatures(LookupRequest lookupRequest)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = this._config.connectionString;
            conn.Open();

            string query = createFeatureLookupQuery(lookupRequest);

            SqlCommand cmd = new SqlCommand();        
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable t = new DataTable("results");
            t.Load(rdr);
            DataSet set = new DataSet();
            set.Tables.Add(t);
            
            if(t.Rows.Count <= 0)
            {
                throw new Exception(string.Format("Error: no tables found for data source"));
            }

            JObject queryResult = JObject.FromObject(set);
            LookupResponse lookupResponse = new LookupResponse();
            lookupResponse.Query = query;
            lookupResponse.Table = lookupRequest.Table;
            JArray features = (JArray)queryResult["results"];
            lookupResponse.Results = features;

            rdr.Close();
            conn.Close();
            return lookupResponse;
        }

        public IList<Table> ListTables()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = this._config.connectionString;
            conn.Open();

            string query = "SELECT * FROM INFORMATION_SCHEMA.TABLES";

            SqlCommand cmd = new SqlCommand();        
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable t = new DataTable("results");
            t.Load(rdr);
            DataSet set = new DataSet();
            set.Tables.Add(t);
            
            if(t.Rows.Count <= 0)
            {
                throw new Exception(string.Format("Error: not tables found for data source"));
            }

            JObject jo = JObject.FromObject(set);
            List<Table> tableList = new List<Table>();
            JArray tables = (JArray)jo["results"];

            foreach (var table in tables)
            {
                Table tbl = new Table();
                tbl.Name = (string)table["TABLE_NAME"];
                tableList.Add(tbl);
            }

            rdr.Close();
            conn.Close();

            return tableList;
        }

        public Table GetTable(string tableName)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = this._config.connectionString;
            conn.Open();

            string query = string.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{0}'", tableName);

            SqlCommand cmd = new SqlCommand();        
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable t = new DataTable("results");
            t.Load(rdr);
            DataSet set = new DataSet();
            set.Tables.Add(t);
            
            if(t.Rows.Count <= 0)
            {
                throw new Exception(string.Format("Error: not tables found for data source"));
            }

            JObject jo = JObject.FromObject(set);
            JArray columns = (JArray)jo["results"];
            Table table = new Table();
            table.Name = tableName;
            table.Features = new List<Feature>();

            foreach(var column in columns)
            {
                Feature feature = new Feature();
                feature.FeatureName = (string)column["COLUMN_NAME"];
                feature.FeatureDataType = (string)column["DATA_TYPE"];
                table.Features.Add(feature);
            }

            rdr.Close();
            conn.Close();

            return table;
        }

        public LookupResponse GetLookupValues(string tableName, string carrierRef, string effectiveDate, string lookupName)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = this._config.connectionString;
            conn.Open();

            string query = createLookupQuery(tableName, carrierRef, effectiveDate, lookupName);

            SqlCommand cmd = new SqlCommand();        
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable t = new DataTable("results");
            t.Load(rdr);
            DataSet set = new DataSet();
            set.Tables.Add(t);
            
            if(t.Rows.Count <= 0)
            {
                throw new Exception(string.Format("Error: no tables found for data source"));
            }

            JObject queryResult = JObject.FromObject(set);
            LookupResponse lookupResponse = new LookupResponse();
            lookupResponse.Query = query;
            lookupResponse.Table = tableName;
            JArray features = (JArray)queryResult["results"];
            lookupResponse.Results = features;

            rdr.Close();
            conn.Close();

            return lookupResponse;
        }


        private string createTargetFeatureSearchQuery(LookupRequest lookupRequest)
        {
            string query = "";
            var end = lookupRequest.LookupFeatures.Last();

            foreach (var feature in lookupRequest.LookupFeatures)
            {
                if(feature.Equals(end))
                {
                    query += feature;
                }
                else
                {
                    query += feature + ", ";
                }
            }
            Log.Information(string.Format("Target Feature Search Query: {0}", query));
            
            return query;
        }

        private string createFeatureFilterQuery (LookupRequest lookupRequest)
        {
            string query = "";
            var end = lookupRequest.LookupFilters.Last();

            foreach (var filter in lookupRequest.LookupFilters)
            {
                if (filter.Equals(end))
                {
                    query += buildFeatureExpression(lookupRequest, filter);
                }
                else
                {
                    query += (buildFeatureExpression(lookupRequest, filter) + " AND ");
                }
            }
            
            Log.Information(string.Format("Target Feature Filter Query: {0}", query));
            return query;
        }

        private string buildFeatureExpression(LookupRequest lookupRequest, LookupFilter filter)
        {
            string expression = string.Format("{0} {1} ",filter.Filter, filter.Operator );
            string dataType = featureDataType(lookupRequest.Table, filter.Filter);
            switch(dataType)
            {
                case "int":
                    expression += string.Format("{0}", Convert.ToInt16(filter.FilterValue));
                    break;
                case "varchar":
                case "date":
                case "datetime":
                    expression += string.Format("'{0}'",filter.FilterValue);
                    break;
                case "float":
                case "double":
                    expression += string.Format("{0}", Convert.ToDouble(filter.FilterValue));
                    break;
                default:
                    expression += string.Format("{0}", filter.FilterValue);
                    break;
            }

            return expression;
        }

        private string featureDataType (string tableName, string featureName)
        {
            Table table = GetTable (tableName);
            foreach(var feature in table.Features)
            {
                if (feature.FeatureName == featureName)
                {
                    return feature.FeatureDataType;
                }
            }

            return "";
        }

        private string createFeatureLookupQuery (LookupRequest lookupRequest)
        {
            
            string featureQuery = createTargetFeatureSearchQuery(lookupRequest);
            string featureFilter = createFeatureFilterQuery(lookupRequest); 
            string query = string.Format("SELECT TOP 1 {0} FROM {1} WHERE {2}", featureQuery, lookupRequest.Table, featureFilter);
            Log.Information("Feature Lookup Query: {0}", query);
            return query;
        }

        private string createLookupQuery(string tableName, string carrierRef, string effectiveDate, string lookupName)
        {
            string [] lookups = lookupName.Split ('|');
            string lookupValues = string.Empty;
            foreach(var lkup in lookups)
            {
                lookupValues += string.Format("{0}",lkup);
                if(lookups.Count() > 1 && lookups.Last() != lkup)
                    lookupValues += ", ";
            }
            string query = string.Format("SELECT DISTINCT {0} FROM {1} WHERE i_company_business_id = {2} and d_effective_date <= '{3}' and b_expired = 0", lookupValues, tableName, lookupCompanyBusinessId(carrierRef), effectiveDate);
            return query;
        }

        private int lookupCompanyBusinessId(string carrierRef)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = this._config.connectionString;
            conn.Open();

            string query = string.Format("SELECT TOP 1 id FROM CompanyBusiness WHERE s_reference_guid = '{0}'",carrierRef);

            SqlCommand cmd = new SqlCommand();        
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable t = new DataTable("results");
            t.Load(rdr);
            DataSet set = new DataSet();
            set.Tables.Add(t);
            
            if(t.Rows.Count <= 0)
            {
                throw new Exception(string.Format("Error: no tables found for data source"));
            }

            rdr.Close();
            conn.Close();
            return Convert.ToInt32(set.Tables[0].Rows[0]["id"]);
            
        }
    }
}