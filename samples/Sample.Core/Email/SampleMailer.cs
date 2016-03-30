using System.Threading.Tasks;
using RazorMailer;
using Sample.Core.Email.Models;

namespace Sample.Core.Email
{
    public class SampleMailer
    {
        private readonly RazorMailerEngine _mailerEngine;

        public SampleMailer() : this(new SmtpDispatcher())
        {
            // Default to the built in Smtp engine
        }

        public SampleMailer(IEmailDispatcher dispatcher)
        {
            _mailerEngine = new RazorMailerEngine(@"email\templates", dispatcher, "hello@example.com", "Sample Website");
        }

        public async Task SendWelcomeEmailAsync(WelcomeModel model)
        {
            var email = _mailerEngine.Create(model.Email, "Welcome to my Example Application", "WelcomePartial", model);
            await _mailerEngine.SendAsync(email);
        }
    }
}
