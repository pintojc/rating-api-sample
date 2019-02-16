using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using IronPython;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace RatingAPI.services.ratingService.providers
{
    class ArchimedesConfig
    {
        public List<string> modules {get; set;}
        public string extensions_directory {get; set;}
        public List<Extension> extensions {get; set; }

        public Repository repo {get; set;}

    }

    public class Extension
    {
        public string name {get;set;}
        public string extension_binary {get; set;}
        public string import_lib {get; set;}
        public List<string> imports {get; set;}
    }

    public class Repository
    {
        public string connection_string {get; set;}
    }

    public class ArchimedesRatingService : IRatingService
    {
        private readonly ArchimedesConfig _config;

        private ScriptEngine engine;
        private ScriptScope scope;

        private JObject result;
        public ArchimedesRatingService(IConfiguration configuration)
        {
            this._config = new ArchimedesConfig();
            configuration.Bind("RatingServices:Archimedes", this._config);
        }

        public string Rate(string transactionType, string quoteData)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> RatingAlgorithms()
        {
            throw new NotImplementedException();
        }

        public string RatingWorksheet(string ratingId)
        {
            throw new NotImplementedException();
        }
    }
}
