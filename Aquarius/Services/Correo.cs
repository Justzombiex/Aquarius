using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;


namespace Aquarius.Services.Services
{
    

    
        public class EmailService
        {
            private readonly IConfiguration _configuration;

            public EmailService(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public void SendEmail(string recipient, string subject, string body)
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");

                using (var client = new SmtpClient(smtpSettings["Server"], int.Parse(smtpSettings["Port"])))
                {
                    client.EnableSsl = bool.Parse(smtpSettings["EnableSsl"]);
                    client.Credentials = new NetworkCredential(smtpSettings["SenderEmail"], smtpSettings["SenderPassword"]);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpSettings["SenderEmail"]),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    };

                    mailMessage.To.Add(recipient);

                    client.Send(mailMessage);
                }
            }
        }
    }

