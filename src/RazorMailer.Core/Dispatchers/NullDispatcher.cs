using System.Net.Mail;
using System.Threading.Tasks;

namespace RazorMailer.Core.Dispatchers
{
    /// <summary>
    /// A special dispatcher that doesn't actually send the MailMessage to a mail server or relay
    /// </summary>
    public class NullDispatcher : IEmailDispatcher
    {
        /// <summary>
        /// Pretends to send a MailMessage
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        public void Send(MailMessage message)
        {
        }

        /// <summary>
        /// Pretends to asynchronously send a MailMessage
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        public Task SendAsync(MailMessage message)
        {
            return Task.FromResult(true);
        }
    }
}