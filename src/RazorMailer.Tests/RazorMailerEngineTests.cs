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
            _mailerEngine = new RazorMailerEngine("templates", "hello@sampleapp.com", "SampleApp", dispatcher.Object);
        }

        [Fact]
        public void simple_template_with_typed_model_test()
        {
            var email = _mailerEngine.Create("WelcomeSimple", new WelcomeModel { Name = "Joe Blogs" }, "joe@blogs.com", "Welcome to our service");
            _mailerEngine.Send(email);

            dispatcher.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }

        [Fact]
        public void simple_template_without_model()
        {
            var email = _mailerEngine.Create("WelcomeSimpleNoModel", "joe@blogs.com", "Welcome to our service");
            _mailerEngine.Send(email);

            dispatcher.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
        }

        [Fact]
        public async Task async_simple_template_with_typed_model_test()
        {
            var email = _mailerEngine.Create("WelcomeSimple", new WelcomeModel { Name = "Joe Blogs" }, "joe@blogs.com", "Welcome to our service");
            await _mailerEngine.SendAsync(email);

            dispatcher.Verify(x => x.SendAsync(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }

        [Fact]
        public void layout_template_with_typed_model_test()
        {
            var email = _mailerEngine.Create("WelcomePartial", new WelcomeModel { Name = "Joe Blogs" }, "joe@blogs.com", "Welcome to our service");
            _mailerEngine.Send(email);

            dispatcher.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }

        [Fact]
        public async Task async_layout_template_with_typed_model_test()
        {
            var email = _mailerEngine.Create("WelcomePartial", new WelcomeModel { Name = "Joe Blogs" }, "joe@blogs.com", "Welcome to our service");
            await _mailerEngine.SendAsync(email);

            dispatcher.Verify(x => x.SendAsync(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }
    }
}
