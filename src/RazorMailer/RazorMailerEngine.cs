using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace RazorMailer
{
    public class RazorMailerEngine : IDisposable
    {
        private readonly string _fromName;
        private readonly string _fromEmail;
        private readonly IEmailDispatcher _dispatcher;
        private readonly IRazorEngineService _service;

        public RazorMailerEngine(string templatePath, IEmailDispatcher dispatcher, string fromEmail, string fromName)
        {
            _fromName = fromName;
            _fromEmail = fromEmail;
            _dispatcher = dispatcher;

            // Find templates in a web application
            var webPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", templatePath);
            // Find templates from a unit test or console application
            var libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templatePath);

            var config = new TemplateServiceConfiguration
            {
                TemplateManager = new ResolvePathTemplateManager(new[] { webPath, libraryPath })
            };

            _service = RazorEngineService.Create(config);
        }

        public MailMessage Create<T>(string to, string subject, string emailTemplate, T model)
        {
            var key = _service.GetKey(emailTemplate);
            var body = _service.RunCompile(key, typeof(T), model);

            return CreateMailMessage(to, subject, body);
        }

        public void Send(MailMessage message)
        {
            _dispatcher.Send(message);
            message.Dispose();
        }

        public async Task SendAsync(MailMessage message)
        {
            await _dispatcher.SendAsync(message);
            message.Dispose();
        }

        private MailMessage CreateMailMessage(string to, string subject, string body)
        {
            var message = new MailMessage();

            message.From = string.IsNullOrEmpty(_fromName) ? new MailAddress(_fromEmail) : new MailAddress(_fromEmail, _fromName);

            message.To.Add(to);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;

            return message;
        }

        public void Dispose()
        {
            _service.Dispose();
        }
    }
}
