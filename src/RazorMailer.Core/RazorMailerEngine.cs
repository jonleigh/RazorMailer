using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace RazorMailer.Core
{
    public class RazorMailerEngine : IDisposable
    {
        private readonly string _fromName;
        private readonly string _fromEmail;
        private readonly IEmailDispatcher _dispatcher;
        private readonly IRazorEngineService _service;

        /// <summary>
        /// Constructs a RazorMailerEngine instance responsible for converting Razor templates into strings only and can't be used to create or send MailMessages.
        /// N.B. As this class loads up templates from the file system, it should only be created once per instance of your application.
        /// </summary>
        /// <param name="templatePath">The path to load the Razor templates from.  e.g. @"email\templates".  The template's Build Action should be set to Content and the Copy to Output Directory flag set to Copy Always</param>
        public RazorMailerEngine(string templatePath) : this(templatePath, null, null, null)
        {
        }

        /// <summary>
        /// Constructs a RazorMailerEngine instance responsible for converting Razor templates into either a MailMessage or string.  This constructor defaults to the build in SmtpDispatcher that takes its settings from the <mailSettings></mailSettings> section within your application config file.
        /// N.B. As this class loads up templates from the file system, it should only be created once per instance of your application.
        /// </summary>
        /// <param name="templatePath">The path to load the Razor templates from.  e.g. @"email\templates".  The template's Build Action should be set to Content and the Copy to Output Directory flag set to Copy Always</param>
        /// <param name="fromEmail">The address the email is from. e.g. hello@yoursite.com</param>
        /// <param name="fromName">The name the email is from. e.g. Your Site</param>
        public RazorMailerEngine(string templatePath, string fromEmail, string fromName) : this(templatePath, fromEmail, fromName, new SmtpDispatcher())
        {
        }

        /// <summary>
        /// Constructs a RazorMailerEngine instance responsible for converting Razor templates into either a MailMessage or string.
        /// N.B. As this class loads up templates from the file system, it should only be created once per instance of your application.
        /// </summary>
        /// <param name="templatePath">The path to load the Razor templates from.  e.g. @"email\templates".  The template's Build Action should be set to Content and the Copy to Output Directory flag set to Copy Always</param>
        /// <param name="fromEmail">The address the email is from. e.g. hello@yoursite.com</param>
        /// <param name="fromName">The name the email is from. e.g. Your Site</param>
        /// <param name="dispatcher">The method by which to send the email.  Custom dispatchers can be implemented using the IEmailDispatcher interface</param>
        public RazorMailerEngine(string templatePath, string fromEmail, string fromName, IEmailDispatcher dispatcher)
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

        /// <summary>
        /// Creates a string from the specified template
        /// </summary>
        /// <param name="template">The name of the Razor template (without the extension)</param>
        /// <returns>A string containing the result of the template</returns>
        public virtual string Create(string template)
        {
            var key = _service.GetKey(template);
            return _service.RunCompile(key);
        }

        /// <summary>
        /// Creates a string from the specified template
        /// </summary>
        /// <param name="template">The name of the Razor template (without the extension)</param>
        /// <param name="model">A typed model containting data for the variables in your Razor template</param>
        /// <returns>A string containing the result of the template and model</returns>
        public virtual string Create<T>(string template, T model)
        {
            var key = _service.GetKey(template);
            return _service.RunCompile(key, typeof(T), model);
        }

        /// <summary>
        /// Creates a MailMessage from the specified template
        /// </summary>
        /// <param name="template">The name of the Razor template (without the extension)</param>
        /// <param name="to">The email address of the person the email is to be sent to</param>
        /// <param name="subject">The email subject</param>
        /// <param name="attachments">Any attachments to be included in the email</param>
        /// <returns>A MailMessage</returns>
        public virtual MailMessage Create(string template, string to, string subject, Attachment[] attachments = null)
        {
            var key = _service.GetKey(template);
            var body = _service.RunCompile(key);

            return CreateMailMessage(to, subject, body, attachments);
        }

        /// <summary>
        /// Creates a MailMessage from the specified template and model
        /// </summary>
        /// <param name="template">The name of the Razor template (without the extension)</param>
        /// <param name="model">A typed model containting data for the variables in your Razor template</param>
        /// <param name="to">The address the email is to be sent to</param>
        /// <param name="subject">The email subject</param>
        /// <param name="attachments">Any attachments to be included in the email</param>
        /// <returns>A MailMessage</returns>
        public virtual MailMessage Create<T>(string template, T model, string to, string subject, Attachment[] attachments = null)
        {
            var key = _service.GetKey(template);
            var body = _service.RunCompile(key, typeof(T), model);

            return CreateMailMessage(to, subject, body, attachments);
        }

        /// <summary>
        /// Sends a MailMessage using the IEmailDispatcher specified in the constructor
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        public virtual void Send(MailMessage message)
        {
            if (_dispatcher == null)
                throw new MissingEmailDispatcherException("This RazorMailerEngine instance was constructed without a IEmailDispatcher and thus can't send MailMessages");

            _dispatcher.Send(message);
            message.Dispose();
        }

        /// <summary>
        /// Sends a MailMessage asynchronously using the IEmailDispatcher specified in the constructor
        /// </summary>
        /// <param name="message">The MailMessage to send</param>
        public virtual async Task SendAsync(MailMessage message)
        {
            if (_dispatcher == null)
                throw new MissingEmailDispatcherException("This RazorMailerEngine instance was constructed without a IEmailDispatcher and thus can't send MailMessages");

            await _dispatcher.SendAsync(message);
            message.Dispose();
        }


        /// <summary>
        /// Creates a MailMessage with the fromEmail and fromName specified in the constructor
        /// </summary>
        /// <param name="to">The email address to whom the email will be addressed</param>
        /// <param name="subject">The subject of the email</param>
        /// <param name="body">The HTML body of the email</param>
        /// <param name="attachments">Any attachments to be included in the email</param>
        /// <returns></returns>
        public virtual MailMessage CreateMailMessage(string to, string subject, string body, Attachment[] attachments = null)
        {
            if (string.IsNullOrEmpty(_fromEmail))
                throw new MissingInformationException("This RazorMailerEngine instance was constructed without a 'From Email' and thus can't send MailMessages");

            if (string.IsNullOrEmpty(_fromEmail))
                throw new MissingInformationException("This RazorMailerEngine instance was constructed without a 'From Name' and thus can't send MailMessages");

            var message = new MailMessage
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = body
            };
            message.To.Add(to);
            message.From = string.IsNullOrEmpty(_fromName) ? new MailAddress(_fromEmail) : new MailAddress(_fromEmail, _fromName);

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }
            }

            return message;
        }

        /// <summary>
        /// Disposes of the underlying RazorEngine service
        /// </summary>
        public void Dispose()
        {
            _service.Dispose();
        }
    }
}
