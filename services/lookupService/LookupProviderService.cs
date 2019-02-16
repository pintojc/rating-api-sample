using System;
using System.Collections;
using System.Collections.Generic;
using RatingAPI.services.lookupService;
using Microsoft.Extensions.Configuration;

namespace RatingAPI.services.lookupService
{   
    
    public class LookupProviderServiceConfig
    {

    }
    public class LookupProviderService : ILookupServiceFactory
    //public class LookupProviderService
    {
        private readonly IConfiguration _configuration;
        public LookupProviderService (IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public  ILookupService CreateLookupService(string serviceName)
        {
            switch (serviceName)
            {
                case "SQL":
                    return new SQLLookupService(this._configuration);
                default:
                    //throw new Exception (string.Format("Error: No service with name {0} found.", serviceName));
                    return null;
            }
        }
    }
}