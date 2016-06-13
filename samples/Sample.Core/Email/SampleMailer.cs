using System.Threading.Tasks;
using RazorMailer;
using Sample.Core.Email.Models;

namespace Sample.Core.Email
{
    public class SampleMailer
    {
        private readonly RazorMailerEngine _mailerEngine;

        public SampleMailer()
        {
            // Default to the built in Smtp engine
            _mailerEngine = new RazorMailerEngine(@"email\templates", "hello@example.com", "Sample Website");
        }

        public async Task SendWelcomeEmailAsync(WelcomeModel model)
        {
            var email = _mailerEngine.Create("WelcomePartial", model, model.Email, "Welcome to my Example Application");
            await _mailerEngine.SendAsync(email);
        }
    }
}
