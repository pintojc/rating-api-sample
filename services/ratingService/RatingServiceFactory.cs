using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RatingAPI.services.ratingService.providers;

namespace RatingAPI.services.ratingService
{
    
    public class RatingServiceFactory : IRatingServiceFactory
    {
        private readonly IConfiguration _configuration;

        public RatingServiceFactory(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public IRatingService CreateRatingService(string serviceName)
        {
            switch(serviceName.ToLower())
            {
                case "archimedes": return new ArchimedesRatingService(this._configuration);
                default: throw new NotImplementedException (string.Format("Error: rating service {0} is not suported.", serviceName.ToLower()));
            }
        }

        public IRatingService CreateRatingEngineService(string line, string state)
        {
            return CreateRatingService("archimedes");
        }
    }
}