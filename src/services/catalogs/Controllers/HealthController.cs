using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TestApp.Controllers
{
  [Route("")]
  public class HealthController : Controller
  {
    [HttpGet("/health")]
    public bool Get()
    {
      return true;
    }
  }
}
