using System;

namespace RatingAPI.services.ratingService
{
    public interface IRatingServiceFactory
    {
        IRatingService CreateRatingService (string serviceName);

        IRatingService CreateRatingEngineService(string line, string state);
        
    }


}