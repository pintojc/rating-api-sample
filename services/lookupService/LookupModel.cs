using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;


namespace RatingAPI.services.lookupService
{
    public class Table
    {
        public string Name {get; set;}
        public IList<Feature> Features {get; set;}
    }

    public class Feature 
    {
        public string FeatureName{get; set;}
        public string FeatureDataType{get; set;}
    }

    public class LookupRequest
    {
        
        [JsonProperty("table")]
        public string Table { get; set; }

        [JsonProperty("lookup_features")]
        public string[] LookupFeatures { get; set; }

        [JsonProperty("lookup_filters")]
        public IList<LookupFilter> LookupFilters { get; set; }
    }

    public class LookupFilter
    {
        [JsonProperty("filter")]
        public string Filter { get; set; }

        [JsonProperty("operator")]
        public string Operator { get; set; }

        [JsonProperty("filter_value")]
        public string FilterValue { get; set; }
        
        [JsonProperty("filter_expression")]
        public string FilterExpression
        {
            get
            {
                return this._createFilterExpression();
            }

            set
            {
                this._filterExpression = value;
            }
        }

        private string  _filterExpression;
        private string _createFilterExpression()
        {
            
            _filterExpression = string.Format("{0} {1} '{2}'", Filter, Operator, FilterValue);
            return _filterExpression;
        }
    }

     public partial class LookupResponse
    {
        [JsonProperty("table")]
        public string Table { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("results")]
        public JArray Results { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("feature_1", NullValueHandling = NullValueHandling.Ignore)]
        public string Feature1 { get; set; }

        [JsonProperty("feature_2", NullValueHandling = NullValueHandling.Ignore)]
        public string Feature2 { get; set; }
    }
}

