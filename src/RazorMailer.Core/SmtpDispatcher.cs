using System.Net.Mail;
using System.Threading.Tasks;

namespace RazorMailer.Core
{
    public class SmtpDispatcher : IEmailDispatcher
    {
        public void Send(MailMessage message)
        {
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
        }

        public async Task SendAsync(MailMessage message)
        {
            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }
        }
    }
}
