using Microsoft.AspNetCore.Mvc;

namespace CiK.Catalog.v1.Controllers {
  [Route ("")]
  public class HealthController : Controller {

    [HttpGet ("/health")]
    public bool Get () {
      return true;
    }

  }
}