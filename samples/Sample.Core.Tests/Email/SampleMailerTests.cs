using System.Net.Mail;
using System.Threading.Tasks;
using Moq;
using RazorMailer.Core.Dispatchers;
using Sample.Core.Email;
using Sample.Core.Email.Models;
using Xunit;

namespace Sample.Core.Tests.Email
{
    public class SampleMailerTests
    {
        private readonly Mock<IEmailDispatcher> _dispatcher;
        private readonly Mailer _mailer;

        public SampleMailerTests()
        {
            // Initialised per test by xunit            
            _dispatcher = new Mock<IEmailDispatcher>();
            _mailer = new Mailer(_dispatcher.Object);
        }

        [Fact]
        public async Task send_welcome_email_async()
        {
            await _mailer.SendWelcomeEmailAsync(new WelcomeModel { Name = "Joe Blogs", Email = "joe@blogs.com" });
            _dispatcher.Verify(x => x.SendAsync(It.Is<MailMessage>(m => m.Subject == "Welcome to my Example Application")), Times.Once);
        }

        [Fact]
        public async Task send_simple_welcome_email_async()
        {
            await _mailer.SendSimpleWelcomeEmailAsync(new WelcomeModel { Name = "Joe Blogs", Email = "joe@blogs.com" });
            _dispatcher.Verify(x => x.SendAsync(It.Is<MailMessage>(m => m.Subject == "Welcome to my Example Application")), Times.Once);
        }

        [Fact]
        public async Task send_cat_email_async()
        {
            await _mailer.SendCatEmailAsync(new WelcomeModel { Name = "Joe Blogs", Email = "joe@blogs.com" });
            _dispatcher.Verify(x => x.SendAsync(It.Is<MailMessage>(m => m.Subject == "Welcome to my feline application")), Times.Once);
        }
    }
}
