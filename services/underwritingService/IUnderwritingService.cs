using System;
using System.Collections;
using System.Collections.Generic;

namespace RatingAPI.services.underwritingService
{
    public interface IUnderwritingService
    {
        List<string> UnderwritingQuestions(string transaction, string level, string lineOfBusiness, string referenceId);
        string Underwrite (string transaction, string level, string referenceId, string questions, string quote);

    }
}