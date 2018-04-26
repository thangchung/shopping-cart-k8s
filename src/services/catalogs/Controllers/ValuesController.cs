using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TestApp.Controllers
{
  [Route("api/[controller]")]
  public class ValuesController : Controller
  {
    // GET api/values
    [HttpGet]
    public IEnumerable<string> Get()
    {
      return new string[] { "value1", "value2" };
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    // POST api/values
    [HttpPost]
    public void Post([FromBody]string value)
    {
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }

    [HttpPost("/email")]
    public void Email(string subject, string body)
    {
      var client = new SmtpClient("localhost");
      client.UseDefaultCredentials = false;
      client.Credentials = new NetworkCredential("username", "password");

      var mailMessage = new MailMessage();
      mailMessage.From = new MailAddress("whoever@me.com");
      mailMessage.To.Add("thangchung@ymail.com");
      mailMessage.Body = body;
      mailMessage.Subject = subject;
      client.Send(mailMessage);
    }
  }
}
