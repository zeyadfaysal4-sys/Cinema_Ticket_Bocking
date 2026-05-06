using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Cinema_Ticket_Bocking.Utiltes
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("zeyadfaysal4@gmail.com", "jnhv mygg wzvg cqmw"),
            };
            var mail = new MailMessage(from: "zeyadfaysal4@gmail.com", to: email, subject, htmlMessage);
            {
                mail.IsBodyHtml = true;
            } 
            ;

            return client.SendMailAsync(mail);
        }
    }
}
