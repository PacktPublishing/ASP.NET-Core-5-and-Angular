using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;

namespace WorldCities.Services
{
    public class MailKitEmailSender : IEmailSender
    {
        public MailKitEmailSender(
            IOptions<MailKitEmailSenderOptions> options
            )
        {
            this.Options = options.Value;
        }

        public MailKitEmailSenderOptions Options { get; set; }

        public Task SendEmailAsync(
            string email, 
            string subject, 
            string message)
        {
            return Execute(email, subject, message);
        }

        public Task Execute(
            string email, 
            string subject, 
            string message)
        {
            // create message
            var msg = new MimeMessage();
            msg.Sender = MailboxAddress.Parse(Options.Sender_EMail);
            if (!string.IsNullOrEmpty(Options.Sender_Name))
                msg.Sender.Name = Options.Sender_Name;
            msg.From.Add(msg.Sender);
            msg.To.Add(MailboxAddress.Parse(email));
            msg.Subject = subject;
            msg.Body = new TextPart(TextFormat.Html) { Text = message };

            // send email
            using (var smtp = new SmtpClient())
            {
                smtp.Connect(
                    Options.Host_Address, 
                    Options.Host_Port, 
                    Options.Host_SecureSocketOptions);
                smtp.Authenticate(
                    Options.Host_Username, 
                    Options.Host_Password);
                smtp.Send(msg);
                smtp.Disconnect(true);
            }

            return Task.FromResult(true);
        }
    }
}
