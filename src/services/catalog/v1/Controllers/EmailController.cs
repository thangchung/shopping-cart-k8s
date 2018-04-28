using System;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CiK.Catalog.v1.Controllers {

  [ApiVersion ("1.0")]
  [Route ("api/v{api-version:apiVersion}/[controller]")]
  public class EmailController : Controller {
    private readonly ILogger _logger;
    public EmailController (ILogger<EmailController> logger) {
      _logger = logger;
    }

    [HttpPost ("")]
    [ProducesResponseType (typeof (string), 200)]
    [ProducesResponseType (404)]
    public string Post (string subject, string body) {
      var host = Environment.GetEnvironmentVariable ("EMAIL_SERVICE_SERVICE_HOST");
      _logger.LogInformation ($"Email: Get HOST from EMAIL_SERVICE_SERVICE_HOST: {host}");

      var port = Environment.GetEnvironmentVariable ("EMAIL_SERVICE_SERVICE_PORT_SMTP");
      _logger.LogInformation ($"Email: Get PORT from EMAIL_SERVICE_SERVICE_PORT_SMTP: {port}");

      // TODO: hard code here
      port = "1025";

      try {
        var client = new SmtpClient ();

        client.Host = host;
        client.Port = Convert.ToInt32 (port);
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential ("username", "password");

        var mailMessage = new MailMessage ();
        mailMessage.From = new MailAddress ("whoever@me.com");
        mailMessage.To.Add ("thangchung@ymail.com");
        mailMessage.Body = body;
        mailMessage.Subject = subject;

        client.Send (mailMessage);
        _logger.LogInformation ("Email: Finished.");
      } catch (System.Exception) {
        // might have runtime exception here
      }

      return $"{host}:{port}";
    }

  }
}