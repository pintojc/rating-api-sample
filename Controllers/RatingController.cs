using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RatingAPI.services.ratingService;

namespace RatingAPI.Controllers
{
    [Route("rater/[controller]/[action]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingServiceFactory _ratingServiceFactory;
        public RatingController(IRatingServiceFactory ratingServiceFactory)
        {
            this._ratingServiceFactory = ratingServiceFactory;
        }
        // GET api/values
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<IDictionary<string, string>> Rates(string line, string state)
        {
            try{

                var ratingEngine = _ratingServiceFactory.CreateRatingService("archimedes");
                Dictionary<string, string> algorithms = (Dictionary<string, string>)ratingEngine.RatingAlgorithms();
                
                if(algorithms.Count == 0 )
                {
                    return NotFound ("No Algorithms found");
                }

                return algorithms;

            }
            catch (Exception ex)
            {
                return NotFound(string.Format ("Error retrieving rates: {0}", ex.Message));
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Rate(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            
        }
    }
}
