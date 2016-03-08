using System.Net.Mail;
using System.Threading.Tasks;
using Moq;
using RazorMailer.Tests.Models;
using Xunit;

namespace RazorMailer.Tests
{
    public class RazorMailerTests
    {
        Mock<IEmailDispatcher> dispatcher;
        RazorMailer mailer;

        public RazorMailerTests()
        {
            // Initialised per test by xunit            
            dispatcher = new Mock<IEmailDispatcher>();
            mailer = new RazorMailer("templates", dispatcher.Object, "hello@sampleapp.com", "SampleApp");
        }

        [Fact]
        public void simple_template_with_typed_model_test()
        {
            var email = mailer.Create("joe@blogs.com", "Welcome to our service", "WelcomeSimple", new WelcomeModel { Name = "Joe Blogs" });
            mailer.Send(email);

            dispatcher.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }

        [Fact]
        public async Task async_simple_template_with_typed_model_test()
        {
            var email = mailer.Create("joe@blogs.com", "Welcome to our service", "WelcomeSimple", new WelcomeModel { Name = "Joe Blogs" });
            await mailer.SendAsync(email);

            dispatcher.Verify(x => x.SendAsync(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }

        [Fact]
        public void layout_template_with_typed_model_test()
        {
            var email = mailer.Create("joe@blogs.com", "Welcome to our service", "WelcomePartial", new WelcomeModel { Name = "Joe Blogs" });
            mailer.Send(email);

            dispatcher.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }

        [Fact]
        public async Task async_layout_template_with_typed_model_test()
        {
            var email = mailer.Create("joe@blogs.com", "Welcome to our service", "WelcomePartial", new WelcomeModel { Name = "Joe Blogs" });
            await mailer.SendAsync(email);

            dispatcher.Verify(x => x.SendAsync(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }
    }
}
