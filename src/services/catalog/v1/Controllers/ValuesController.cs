using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace CiK.Catalog.v1.Controllers {
  [Route ("api/v{api-version:apiVersion}/[controller]")]
  [ApiVersion ("1.0")]
  public class ValuesController : Controller {
    // GET api/values
    [HttpGet]
    public IEnumerable<string> Get () {
      return new string[] { "value1", "value2" };
    }

    // GET api/values/5
    [HttpGet ("{id}")]
    public string Get (int id) {
      return "value";
    }

    // POST api/values
    [HttpPost]
    public void Post ([FromBody] string value) { }

    // PUT api/values/5
    [HttpPut ("{id}")]
    public void Put (int id, [FromBody] string value) { }

    // DELETE api/values/5
    [HttpDelete ("{id}")]
    public void Delete (int id) { }
  }
}