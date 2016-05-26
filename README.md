##RazorMailer## 
![Build Status](https://westridgedesign.visualstudio.com/_apis/public/build/definitions/821eded2-7e35-482d-9589-e62425bf523a/2/badge "Build Status")



RazorMailer is a lightweight framework, based on RazorEngine, that makes it really easy to send emails using Razor templates.  The reason for its existence and information on how it was built can be found on [my blog](http://jonleigh.me/creating-a-new-email-framework-for-dot-net/).  It has been designed so that it doesn't rely on any one framework, allowing you to send emails from your preferred host, be it a console or service application, ASP.NET MVC, NancyFX or batch processing frameworks such as Azure WebJobs or Hangfire.

It supports the following:

* Model based Razor templates using POCOs
* Template caching
* Layouts
* Unit testing
* Sending of emails via 3rd party mechanisms by extending ``IEmailDispatcher``


A [sample application](https://github.com/jonleigh/RazorMailer/tree/master/samples) has been provided demonstrating a working implementation of RazorMailer.  I've also included some information on two tools that come in handy when developing and testing emails within a project.  This can be found near the [bottom of the page](#development-and-testing).

###Models###
At present, RazorMailer only supports typed POCO models to populate templates.  This means you will need to create a model per email or share the same model across emails of a similar nature (e.g both only need a link to your website).  While it's easy to get up and running with ``dynamic`` models, their use in this regard is a code smell and the lack of type checking can soon become a maintenance headache.

###Core Logic###
RazorMailer can be best used in one of two ways:

1. Centralising your email logic into a single email class, responsible for sending every email within your solution.
2. Writing a separate class per email

Option 1 is normally adequate but if you're paying strict attention to the single responsibility principle, you may want to choose option 2.  Whatever you choose, just ensure you have adequate unit test coverage of your email logic.  A sample centralised email class can be found below:

```csharp
public class SampleMailer
{
	private readonly RazorMailerEngine _mailerEngine;

	public SampleMailer() : this(new SmtpDispatcher())
	{
		// Default to the built in Smtp engine
	}

	public SampleMailer(IEmailDispatcher dispatcher)
	{
		_mailerEngine = new RazorMailerEngine(@"email\templates", dispatcher, "hello@example.com", "Sample Website");
	}

	public async Task SendWelcomeEmailAsync(WelcomeModel model)
	{
		var email = _mailerEngine.Create(model.Email, "WelcomePartial", "Welcome to my Example Application", model);
		await _mailerEngine.SendAsync(email);
	}
}
```	

###Templates###

Templates can be included within your host project (e.g a console or ASP.NET MVC application) or within a seperate class library within your solution, such as a "Core" library.  The benefits of hosting within said library are twofold a) being we can share the templates across different application types and b) that we can cover the templates with unit tests.

N.B The templates can be included in an project within your solution but must have their ``Build Action`` set to ``Content`` and ``Copy to Output Directory`` flag set to ``Copy always``.  This ensures that the templates end up the in the same folder as the executing assembly and are included when you package your application.

A simple template (without a layout) is included below:

```csharp
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html lang="en">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <meta name="viewport" content="initial-scale=1.0">    <!-- So that mobile webkit will display zoomed in -->
    <meta name="format-detection" content="telephone=no"> <!-- disable auto telephone linking in iOS -->
</head>

<body>
    <p>Hi @Model.Name,</p>

    <p>Welcome to my website.</p>

    <p>Have a nose around and let me know what you think!</p>

    <p>Toodles</p>
</body>
</html>
```


###Template Caching###

When a template is first called, the template is loaded up from disk, compiled and cached into memory for future calls.  It is therefore essential that you only construct the ``RazorMailer`` class once in your application (e.g through the use of a container such as SimpleInjector) or you run the risk of running into a severe performance issue.

###Layouts###
Layouts can be added by including them in the same folder as your template and referencing them at the top though a directive at the top of the template.  The layout will automatically be picked up, compiled and cached for quick reuse on the next call.

A layout and partial template are included below:

```csharp
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html lang="en">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <meta name="viewport" content="initial-scale=1.0">    <!-- So that mobile webkit will display zoomed in -->
    <meta name="format-detection" content="telephone=no"> <!-- disable auto telephone linking in iOS -->
</head>

<body>
    @RenderBody()
</body>
</html>
```

```csharp
@{
    Layout = "_WelcomeLayout";
}

<p>Hi @Model.Name,</p>

<p>Welcome to my website.</p>

<p>Have a nose around and let me know what you think!</p>

<p>Toodles</p>
```

###Sending Emails###
The actual sending of an email is done through the ``IEmailDispatcher`` interface allowing you to either use the built in ``SmtpDispatcher`` (which utilises the built in .NET classes) or write your own, most probably because you would like to use the API of your favourite transactional email gateway such as MailJet, Mandrill etc.  It also handily leads into our next point:

###Unit Testing###
It's fairly trivial to write unit tests for specific emails by mocking out the ``IEmailDispatcher`` interface (here using Moq), creating a model and calling the Create method on our RazorMailer class.  A simple test is outlined below:

```csharp
var dispatcher = new Mock<IEmailDispatcher>();
var mailer = new RazorMailer("templates", dispatcher.Object, "hello@sampleapp.com", "SampleApp");

var email = mailer.Create("joe@blogs.com", "WelcomePartial", "Welcome to our service", new WelcomeModel { Name = "Joe Blogs" });
mailer.Send(email);

dispatcher.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Once);
Assert.Contains("Joe Blogs", email.Body);
```

###Development and Testing###
The included [sample application](https://github.com/jonleigh/RazorMailer/tree/master/samples) makes use of a simple SMTP client for windows called [smtp4dev](http://smtp4dev.codeplex.com/). smtp4dev sits in your tray and collects any emails sent to it on port 25, no matter who they're addressed to.

Another option is to use the test email server in a box solution, [Mailtrap](https://mailtrap.io/). Mailtrap provides you with a test mailbox in the cloud that traps all emails sent to it, preventing them being delivered to the end email address. This is especially handy if you want to interact with emails sent from a test environment whilst simultaneously wanting avoiding those embarrassing moments when a user(s) receives an email they shouldn't have.


