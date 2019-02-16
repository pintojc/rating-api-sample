using System;
using System.Collections;
using System.Collections.Generic;

namespace RatingAPI.services.lookupService
{
    public interface ILookupServiceFactory
    {
        ILookupService CreateLookupService(string serviceName);
    }
}