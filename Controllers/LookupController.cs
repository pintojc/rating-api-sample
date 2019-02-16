using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RatingAPI.services.lookupService;
using Serilog;

namespace RatingAPI.Controllers
{
    [Route("rater/[controller]/[action]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ILookupServiceFactory _lookupProviders;
        private readonly IConfiguration _configuration;

        public LookupController(IConfiguration configuration, ILookupServiceFactory lookupProviders)
        {
            this._configuration = configuration;
            _lookupProviders = lookupProviders;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<List<Table>> Tables ()
        {
            try
            {
                ILookupService service = this._lookupProviders.CreateLookupService("SQL");
                return (List<Table>)service.ListTables();
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Error: {0}", ex.Message));
                return NotFound();
            }

            
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Table> Table ([FromQuery] string tableName )
        {
            try
            {

                ILookupService service = this._lookupProviders.CreateLookupService("SQL");

                return service.GetTable(tableName);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Table Error: table{0}; error: {0}",tableName, ex.Message));
                return NotFound();
            }

            
        }



        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public ActionResult<LookupResponse> LookupData([FromBody] LookupRequest lookupRequest)
        {
            try
            {
                ILookupService service = this._lookupProviders.CreateLookupService("SQL");
                return service.GetFeatures(lookupRequest);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Error: {0}", ex.Message));
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public ActionResult<LookupResponse> LookupValues([FromQuery] string tableName, string carrierRef, string effectiveDate, string lookupName )
        {
            try
            {
                ILookupService service = this._lookupProviders.CreateLookupService("SQL");
                return service.GetLookupValues(tableName, carrierRef, effectiveDate, lookupName);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("LookupValues Error: {0}", ex.Message));
                return NotFound();
            }
        }
    }

}