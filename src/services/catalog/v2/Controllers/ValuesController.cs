using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace CiK.Catalog.v2.Controllers
{
  [Route("api/v{api-version:apiVersion}/[controller]")]
  [ApiVersion("2.0")]
  public class ValuesController : Controller
  {
    // GET api/values
    [HttpGet]
    public IEnumerable<string> Get()
    {
      return new string[] { "value1 - version 2", "value2 - version 2" };
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value - version 2";
    }
  }
}