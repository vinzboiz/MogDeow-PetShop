using System.Net.Mail;
using System.Net;

namespace MOGDEOW.Helpers
{
    public static class EmailHelper
    {
        public static async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("dloker171202@gmail.com", "bkhlywdubivkfadl"),
                EnableSsl = true,
            };

            var mail = new MailMessage
            {
                From = new MailAddress("dloker171202@gmail.com"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mail.To.Add(to);

            await smtpClient.SendMailAsync(mail);
        }
    }
}
