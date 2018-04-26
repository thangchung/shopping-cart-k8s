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
  public class EmailController : Controller
  {
    [HttpPost("")]
    public bool Post(string subject, string body)
    {
      try
      {
        var client = new SmtpClient("smtp://email-service:25");
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential("username", "password");

        var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress("whoever@me.com");
        mailMessage.To.Add("thangchung@ymail.com");
        mailMessage.Body = body;
        mailMessage.Subject = subject;
        client.Send(mailMessage);
      }
      catch (System.Exception)
      {
        // might have runtime exception here
      }

      return true;
    }
  }
}
