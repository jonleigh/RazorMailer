using System;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using RazorMailer.Core;
using RazorMailer.Tests.Models;
using Xunit;

namespace RazorMailer.Tests
{
    public class RazorMailerEngineTests
    {
        readonly Mock<IEmailDispatcher> _dispatcher;
        RazorMailerEngine _mailerEngine;

        public RazorMailerEngineTests()
        {
            // Initialised per test by xunit            
            _dispatcher = new Mock<IEmailDispatcher>();
            _mailerEngine = new RazorMailerEngine("templates", "hello@sampleapp.com", "SampleApp", _dispatcher.Object);
        }

        [Fact]
        public void mailmessage_simple_template_with_typed_model_test()
        {
            var email = _mailerEngine.Create("WelcomeSimple", new WelcomeModel { Name = "Joe Blogs" }, "joe@blogs.com", "Welcome to our service");
            _mailerEngine.Send(email);

            _dispatcher.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
            Assert.Contains("Joe Blogs", email.Body);
        }

        [Fact]
        public void mailmessage_simple_template_without_model()
        {
            var email = _mailerEngine.Create("WelcomeSimpleNoModel", "joe@blogs.com", "Welcome to our service");
            Assert.Contains("Welcome to my website", email.Body);
        }

        [Fact]
        public void mailmessage_layout_template_with_typed_model()
        {
            var email = _mailerEngine.Create("WelcomePartial", new WelcomeModel { Name = "Joe Blogs" }, "joe@blogs.com", "Welcome to our service");
            Assert.Contains("Joe Blogs", email.Body);
        }
        
        [Fact]
        public void mailmessage_layout_template_with_typed_model_and_attachment()
        {
            var assembly = Assembly.GetAssembly(typeof (RazorMailerEngineTests));
            using (var stream = assembly.GetManifestResourceStream($"RazorMailer.Tests.Resources.GrumpyCat.jpg"))
            {
                var attachment = new Attachment(stream, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                var email = _mailerEngine.Create("WelcomePartial", new WelcomeModel {Name = "Joe Blogs"}, "joe@blogs.com", "Welcome to our service", new[] {attachment});
                Assert.Equal(1, email.Attachments.Count);
                Assert.Equal(attachment, email.Attachments[0]);
            }
        }

        [Fact]
        public void mailmessage_layout_template_without_model_and_attachment()
        {
            var assembly = Assembly.GetAssembly(typeof(RazorMailerEngineTests));
            using (var stream = assembly.GetManifestResourceStream($"RazorMailer.Tests.Resources.GrumpyCat.jpg"))
            {
                var attachment = new Attachment(stream, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                var email = _mailerEngine.Create("WelcomePartialNoModel", "joe@blogs.com", "Welcome to our service", new[] { attachment });
                Assert.Equal(1, email.Attachments.Count);
                Assert.Equal(attachment, email.Attachments[0]);
            }
        }

        [Fact]
        public void string_simple_template_without_model()
        {
            var text = _mailerEngine.Create("WelcomeSimpleNoModel");
            Assert.Contains("Toodles", text);
        }

        [Fact]
        public void string_simple_template_with_typed_model()
        {
            var text = _mailerEngine.Create("WelcomeSimple", new WelcomeModel { Name = "Joe Blogs" });
            Assert.Contains("Joe Blogs", text);
        }

        [Fact]
        public void string_layout_template_with_typed_model_test()
        {
            var text = _mailerEngine.Create("WelcomePartial", new WelcomeModel { Name = "Joe Blogs" });
            Assert.Contains("Joe Blogs", text);
        }

        [Fact]
        public void mailmessage_send()
        {
            var email = _mailerEngine.Create("WelcomePartial", new WelcomeModel { Name = "Joe Blogs" }, "joe@blogs.com", "Welcome to our service");
            _mailerEngine.Send(email);

            _dispatcher.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
        }

        [Fact]
        public async Task async_mailmessage_send()
        {
            var email = _mailerEngine.Create("WelcomePartial", new WelcomeModel { Name = "Joe Blogs" }, "joe@blogs.com", "Welcome to our service");
            await _mailerEngine.SendAsync(email);

            _dispatcher.Verify(x => x.SendAsync(It.IsAny<MailMessage>()), Times.Once);
        }

        [Fact]
        public async Task implicit_ctor_smtp_host_not_provided()
        {
            _mailerEngine = new RazorMailerEngine("templates", "hello@sampleapp.com", "SampleApp");
            var email = _mailerEngine.Create("WelcomePartial", new WelcomeModel { Name = "Joe Blogs" }, "joe@blogs.com", "Welcome to our service");
            await Assert.ThrowsAsync<InvalidOperationException>(() => _mailerEngine.SendAsync(email));
        }

        [Fact]
        public void reduced_ctor_able_to_create_string()
        {
            _mailerEngine = new RazorMailerEngine("templates");
            var text = _mailerEngine.Create("WelcomePartial", new WelcomeModel { Name = "Joe Blogs" });
            Assert.Contains("Joe Blogs", text);
        }

        [Fact]
        public void reduced_ctor_not_able_to_create_mailmessage()
        {
            _mailerEngine = new RazorMailerEngine("templates");
            Assert.Throws<MissingInformationException>(() => _mailerEngine.Create("WelcomePartial", new WelcomeModel { Name = "Joe Blogs" }, "joe@blogs.com", "Welcome to our service"));
        }
    }
}
