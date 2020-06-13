using Cross.Abstractions;
using Cross.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Cross
{
    public class EmailSender : IEmailSender
    {
        private readonly IOptions<EmailSettings> _settings;

        public EmailSender(IOptions<EmailSettings> settings)
        {
            _settings = settings;
        }

        public async Task<bool> Send(string To, string Subject, string Body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(_settings.Value.SmtpClient);
            mail.From = new MailAddress(_settings.Value.Address, _settings.Value.DisplayName);
            mail.To.Add(To);
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = false;

            //System.Net.Mail.Attachment attachment;
            // attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
            // mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            //SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential(_settings.Value.Username, _settings.Value.Password);
            SmtpServer.EnableSsl = true;

            try
            {
                await SmtpServer.SendMailAsync(mail);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            } 
        }
    }
}
