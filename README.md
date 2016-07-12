##RazorMailer##
![Build Status](https://westridgedesign.visualstudio.com/_apis/public/build/definitions/821eded2-7e35-482d-9589-e62425bf523a/2/badge "Build Status")

**RazorMailer** is a lightweight framework, based on RazorEngine, that makes it really easy to send emails using Razor templates.  The reason for its existence and information on how it was built can be found in this [blog post](http://jonleigh.me/creating-a-new-email-framework-for-dot-net/).  

It has been designed so that it doesn't rely on any one framework, allowing you to send emails from your preferred host, be it ASP.NET MVC, NancyFX, a console or service application or a batch processing framework such as Azure WebJobs or Hangfire.

It supports the following:

* Model based Razor templates using POCOs
* Template caching
* Layouts
* Attachments
* Unit testing & mocking
* Sending of emails via 3rd party mechanisms by extending ``IEmailDispatcher``

The extracts below have been taken from a [sample application](https://github.com/jonleigh/RazorMailer/tree/master/samples) which demonstrates a working implementation of RazorMailer using ASP.NET MVC.

Information on two tools, [smtp4dev](http://smtp4dev.codeplex.com/) & [Mailtrap](https://mailtrap.io/)), that come in handy when developing and testing emails in different environments, has been included near the [bottom of the page](#development-and-testing).

###Installation###

**RazorMailer** can be installed from the NuGet Package Manager or via the NuGet Console by typing ``Install-Package RazorMailer``

###Models###
At present, RazorMailer only supports typed POCO models to populate templates.  This means you will need to create a model per email or share the same model across emails of a similar nature (e.g both only need a link to your website).  While it's easy to get up and running with ``dynamic`` models, their use in this regard is a code smell and the lack of type checking can soon become a maintenance headache.

###Core Logic###
RazorMailer can be best used in one of two ways:

1. Centralising your email logic into a single email class, responsible for sending every email within your solution
2. Writing a separate class per email

Option 1 is normally adequate but if you're paying strict attention to the single responsibility principle, you may want to choose option 2.  Whatever you choose, just ensure you have adequate unit test coverage of your email logic.

RazorMailer operates in a two step process.  First, the template is compiled (optionally with a model) into a .NET MailMessage object using the ``Create`` method on ``RazorMailerEngine`` with the default from name and from email specified in the ``RazorMailerEngine`` constructor.  Once the MailMessage has been generated, the ``Send`` or ``SendAsync`` method needs to be called to dispatch the email.  Since RazorMailer generates and works with the built in .NET MailMessage class, you're able to customise the MailMessage to your hearts content before calling the ``Send`` or ``SendAsync`` method.

A ``Mailer`` class from the [sample application](https://github.com/jonleigh/RazorMailer/tree/master/samples), based on option 1, can be found below:

```csharp
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

	/// <summary>
	/// A simple method that asynchronously send a email based on a Razor layout and partial 
	/// </summary>
	public async Task SendWelcomeEmailAsync(WelcomeModel model)
	{
		MailMessage email = _mailerEngine.Create("WelcomePartial", model, model.Email, "Welcome to my Example Application");
		await _mailerEngine.SendAsync(email);
	}
	
	...
}
```	

###Templates###

Templates can be included within your host project (e.g a console or ASP.NET MVC application) or within a seperate class library within your solution, such as a "Core" library.  The benefits of hosting within said library are twofold a) being we can share the templates across different application types and b) that we can cover the templates with [unit tests](#unit-testing).

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

When a template is first called, the template is loaded up from disk, compiled and cached into memory for future calls.  It is therefore essential that you only construct the ``Mailer`` class once in your application (ideally through the use of a container such as SimpleInjector) or you run the risk of running into a severe performance issue.

###Layouts###
Layouts can be added by including them in the same folder as your partial template (the segment to be contained within the layout) and referencing it though a directive at the top of the partial.  The layout will automatically be picked up, compiled and cached for quick reuse on the next call.

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

###Attachments###

RazorMailer uses the build in .NET Attachment class in the Mail namespace.  Simply create attachments as you would if you were using native .NET and pass them into the ``Create`` method on the ``RazorMailerEngine``.  A sample has been provided below:


```csharp
public async Task SendCatEmailAsync(WelcomeModel model)
{
	var assembly = Assembly.GetAssembly(typeof(Mailer));
	using (var stream = assembly.GetManifestResourceStream($"Sample.Core.Email.Resources.GrumpyCat.jpg"))
	{
		if (stream == null)
			throw new Exception("Grumpy cat picture not found");

		var attachment = new Attachment(stream, "GrumpyCat.jpg", System.Net.Mime.MediaTypeNames.Image.Jpeg);
		MailMessage email = _mailerEngine.Create("WelcomeFeline", model, model.Email, "Welcome to my feline application", new[] { attachment });
		await _mailerEngine.SendAsync(email);
	}
}
```

###Sending Emails###
The actual sending of an email is done through the ``IEmailDispatcher`` interface allowing you to either use the built in ``SmtpDispatcher`` (which utilises the built in .NET SmtpClient class) or by writing your own, most probably because you would like to use the API of your favourite transactional email gateway such as MailJet, Mandrill etc.  It also handily leads into our next point:

###Unit Testing###
It's fairly trivial to write unit tests for specific emails by mocking out the ``IEmailDispatcher`` interface (here using Moq), creating a model and calling the method to generate and *send* and email.  

Writing tests in this manner ensures that the template and model can sucessfully be compiled without actually sending the email.

A simple test class is outlined below (more can be found in the [sample application](https://github.com/jonleigh/RazorMailer/tree/master/samples):

```csharp
public class MailerTests
{
	private readonly Mock<IEmailDispatcher> _dispatcher;
	private readonly Mailer _mailer;

	public MailerTests()
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
	
	...
}
```

###Development and Testing###
The included [sample application](https://github.com/jonleigh/RazorMailer/tree/master/samples) makes use of a simple SMTP client for windows called [smtp4dev](http://smtp4dev.codeplex.com/). smtp4dev sits in your tray and collects any emails sent to it on port 25, no matter who they're addressed to.

Another option is to use the test email server in a box solution, [Mailtrap](https://mailtrap.io/). Mailtrap provides you with a test mailbox in the cloud that traps all emails sent to it, preventing them being delivered to the end email address. This is especially handy if you want to interact with emails sent from a test environment whilst simultaneously wanting avoiding those embarrassing moments when a user(s) receives an email they shouldn't have.


