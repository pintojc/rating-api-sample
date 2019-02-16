using System;


namespace RatingAPI.services.underwritingService
{
    public interface IUnderwritingServiceFactory
    {
        IUnderwritingService CreateUnderwritingService (string referenceId);
    }
}