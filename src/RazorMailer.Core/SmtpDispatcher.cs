using System.Net.Mail;
using System.Threading.Tasks;

namespace RazorMailer.Core
{
    /// <summary>
    /// A simple dispatcher that sends emails using the built in .NET SmtpClient with default settings (i.e using configuration file settings)
    /// </summary>
    public class SmtpDispatcher : IEmailDispatcher
    {
        /// <summary>
        /// Sends a MailMessage using a default instance of the .NET Smtp Client
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        public void Send(MailMessage message)
        {
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
        }

        /// <summary>
        /// Sends a MailMessage asynchronously using a default instance of the .NET Smtp Client
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        public async Task SendAsync(MailMessage message)
        {
            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }
        }
    }
}
