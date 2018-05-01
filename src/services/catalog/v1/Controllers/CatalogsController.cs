using System.Collections.Generic;
using System.Threading.Tasks;
using CiK.Fw.Infrastructure.AspNetCore;
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

      var serviceUrl = _env.IsDevelopment() ? "http://localhost:3000/" : "http://supplier:3000/";

      var result = await _rest.GetAsync<List<SupplierModel>>(serviceUrl);
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