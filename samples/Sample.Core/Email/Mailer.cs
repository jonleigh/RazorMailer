using System.Threading.Tasks;
using RazorMailer.Core;
using Sample.Core.Email.Models;

namespace Sample.Core.Email
{
    public class Mailer
    {
        private readonly RazorMailerEngine _mailerEngine;

        /// <summary>
        /// The default constructor that initialises RazorMailer with it's built in SmtpDispatcher
        /// </summary>
        public Mailer() : this(new SmtpDispatcher())
        {
        }

        /// <summary>
        /// This overload allows us to pass in a mock dispatcher for testing purposes
        /// </summary>
        /// <param name="dispatcher"></param>
        public Mailer(IEmailDispatcher dispatcher)
        {
            // Default to the built in Smtp engine
            _mailerEngine = new RazorMailerEngine(@"email\templates", "hello@example.com", "Sample Website", dispatcher);
        }

        public async Task SendWelcomeEmailAsync(WelcomeModel model)
        {
            var email = _mailerEngine.Create("WelcomePartial", model, model.Email, "Welcome to my Example Application");
            await _mailerEngine.SendAsync(email);
        }
    }
}
