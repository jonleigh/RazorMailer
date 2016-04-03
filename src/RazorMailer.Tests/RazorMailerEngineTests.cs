using System.Net.Mail;
using System.Threading.Tasks;
using Moq;
using RazorMailer.Tests.Models;
using Xunit;

namespace RazorMailer.Tests
{
    public class RazorMailerEngineTests
    {
        Mock<IEmailDispatcher> dispatcher;
        RazorMailerEngine _mailerEngine;

        public RazorMailerEngineTests()
        {
            // Initialised per test by xunit            
            dispatcher = new Mock<IEmailDispatcher>();
            _mailerEngine = new RazorMailerEngine("templates", dispatcher.Object, "hello@sampleapp.com", "SampleApp");
        }

        [Fact]
        public void simple_template_with_typed_model_test()
        {
            var email = _mailerEngine.Create("joe@blogs.com", "WelcomeSimple", "Welcome to our service", new WelcomeModel { Name = "Joe Blogs" });
            _mailerEngine.Send(email);

            dispatcher.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }

        [Fact]
        public async Task async_simple_template_with_typed_model_test()
        {
            var email = _mailerEngine.Create("joe@blogs.com", "WelcomeSimple", "Welcome to our service", new WelcomeModel { Name = "Joe Blogs" });
            await _mailerEngine.SendAsync(email);

            dispatcher.Verify(x => x.SendAsync(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }

        [Fact]
        public void layout_template_with_typed_model_test()
        {
            var email = _mailerEngine.Create("joe@blogs.com", "WelcomePartial", "Welcome to our service", new WelcomeModel { Name = "Joe Blogs" });
            _mailerEngine.Send(email);

            dispatcher.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }

        [Fact]
        public async Task async_layout_template_with_typed_model_test()
        {
            var email = _mailerEngine.Create("joe@blogs.com", "WelcomePartial", "Welcome to our service", new WelcomeModel { Name = "Joe Blogs" });
            await _mailerEngine.SendAsync(email);

            dispatcher.Verify(x => x.SendAsync(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }
    }
}
