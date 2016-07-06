using System.Net.Mail;
using System.Threading.Tasks;

namespace RazorMailer.Core.Dispatchers
{
    /// <summary>
    ///     RazorMailer uses the IEmailDispatcher interface to send emails.  By default, the built in SmtpDispatcher is used
    ///     but this interface can be extended to use other methods, such as the API for MailJet, Mandrill etc
    /// </summary>
    public interface IEmailDispatcher
    {
        /// <summary>
        ///     Sends a MailMessage
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        void Send(MailMessage message);

        /// <summary>
        ///     Sends a MailMessage asynchronously
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        Task SendAsync(MailMessage message);
    }
}