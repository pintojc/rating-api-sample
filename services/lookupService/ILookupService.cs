using System;
using System.Collections;
using System.Collections.Generic;
using RatingAPI.services.lookupService;


namespace RatingAPI.services.lookupService
{
    public interface ILookupService
    {
        IList<Table> ListTables();

        Table GetTable(string tableName);
        LookupResponse GetFeatures(LookupRequest lookupRequest);

        LookupResponse GetLookupValues (string tableName, string carrierRef, string effectiveDate, string lookupName);
    }

}