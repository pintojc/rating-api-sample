using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rules;

namespace TodoApi.Controllers
{
    [Route("rater/[controller]/[action]")]
    [ApiController]
    public class RulesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Rules()
        {
           return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}", Name="rule")]
        public ActionResult<string> Rule(int id)
        {
            rule rl = new rule();
            return rl.createEngine();
        }

        // POST api/values
        [HttpPost]
        public ActionResult<string> RuleResult([FromBody] dynamic value)
        {   
            return @"post result: {0}" + value;
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
