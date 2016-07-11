using System;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using RazorMailer.Core;
using RazorMailer.Core.Dispatchers;
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

        public async Task SendSimpleWelcomeEmailAsync(WelcomeModel model)
        {
            var email = _mailerEngine.Create("WelcomeSimple", model, model.Email, "Welcome to my Example Application");
            await _mailerEngine.SendAsync(email);
        }

        public async Task SendCatEmailAsync(WelcomeModel model)
        {
            var assembly = Assembly.GetAssembly(typeof(Mailer));
            using (var stream = assembly.GetManifestResourceStream($"Sample.Core.Email.Resources.GrumpyCat.jpg"))
            {
                if (stream == null)
                    throw new Exception("Grumpy cat picture not found");

                var attachment = new Attachment(stream, "GrumpyCat.jpg", System.Net.Mime.MediaTypeNames.Image.Jpeg);
                var email = _mailerEngine.Create("WelcomeFeline", model, model.Email, "Welcome to my feline application", new[] { attachment });
                await _mailerEngine.SendAsync(email);
            }
        }
    }
}
