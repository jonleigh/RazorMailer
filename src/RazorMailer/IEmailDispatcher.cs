using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RazorMailer
{
    public interface IEmailDispatcher
    {
        void Send(MailMessage message);
        Task SendAsync(MailMessage message);
    }
}
