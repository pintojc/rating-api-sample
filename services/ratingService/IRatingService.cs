using System;
using System.Collections;
using System.Collections.Generic;

namespace RatingAPI.services.ratingService
{
    public interface IRatingService
    {
        IDictionary<string, string> RatingAlgorithms();
        string Rate (string transactionType, string quoteData);

        string RatingWorksheet (string ratingId);
    }
}
