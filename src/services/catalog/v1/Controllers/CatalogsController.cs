using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CiK.Catalog.v1.Controllers
{
  [Route("api/v{api-version:apiVersion}/[controller]")]
  [ApiVersion("1.0")]
  public class CatalogsController : Controller
  {
    private static RestClient _rest;
    private static IHostingEnvironment _env;
    public CatalogsController(RestClient rest, IHostingEnvironment env)
    {
      _rest = rest;
      _env = env;
    }

    [HttpGet]
    public async Task<IEnumerable<SupplierModel>> Get()
    {
      _rest.SetOpenTracingInfo(HttpContext.Request.GetOpenTracingInfo());

      var serviceName = "localhost";
      var port = 3000;

      if (!_env.IsDevelopment())
      {
        serviceName = "supplier";
        port = 3000;
      }

      var result = await _rest.GetAsync<List<SupplierModel>>(serviceName, port, "");
      return result;
    }
  }

  public class SupplierModel
  {
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public string ContactName { get; set; }
    public string ContactTitle { get; set; }
  }
}