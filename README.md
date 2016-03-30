RazorMailer is a lightweight framework, based on RazorEngine, that makes it really easy to send emails using Razor templates.  The reason for its existence and information on how it was built can be found on [my blog](http://jonleigh.me/creating-a-new-email-framework-for-dot-net/).  It has been designed so that it doesn't rely on any one framework, allowing you to send emails from your preferred host, be it a console or service application, ASP.NET MVC, NancyFX or batch processing frameworks such as Azure WebJobs or Hangfire.

It supports the following:

* Model based Razor templates using POCOs
* Template caching
* Layouts
* Unit testing
* Sending of emails via 3rd party mechanisms by extending ``IEmailDispatcher``


A [sample application]() has been provided demonstrating a working implementation of RazorMailer.  I've also included some information on two tools that come in handy when developing and testing emails within a project.  This can be found near the [bottom of the page](#Development-and-Testing).

If you have any queries, comments or suggestions, please get in contact through <contact details>.

###Models###
At present, RazorMailer only supports typed POCO models to populate templates.  This means you will need to create a model per email or share the same model across emails of a similar nature (e.g both only need a link to your website).  While it's easy to get up and running with ``dynamic`` models, their use in this regard is a code smell and the lack of type checking can soon become a maintenance headache.

###Core Logic###
RazorMailer can be best used in one of two ways:

1. Though centralising your email logic into a single email class, responsible for sending every email within your solution.
2. Generating and sending the email within the method that uses it (e.g the reset password method on the ``AuthenticationController``.


I generally prefer option 1 as it's closer to the single responsibility principle than 2, although you could go further and have a single class per each email.  Whatever you choose, just ensure you have adequate unit test coverage of your email logic.  A sample centralised email class can be found below:

<script src="https://gist.github.com/jonleigh/172c52aca36ca5b89a75c018262c3822.js"></script>

###Templates###

Templates can be included within your host project (e.g a console or ASP.NET MVC application) or within a seperate class library within your solution, such as a "Core" library.  The benefits of hosting within said library are twofold a) being we can share the templates across different application types and b) that we can cover the templates with unit tests.

N.B The templates can be included in an project within your solution but must have their ``Build Action`` set to ``Content`` and ``Copy to Output Directory`` flag set to ``Copy always``.  This ensures that the templates end up the in the same folder as the executing assembly and are included when you package your application.

A simple template (without a layout) is included below:

<script src="https://gist.github.com/jonleigh/5f88edd0d1a10589bce9.js"></script>


###Template Caching###

When a template is first called, the template is loaded up from disk, compiled and cached into memory for future calls.  It is therefore essential that you only construct the ``RazorMailer`` class once in your application (e.g through the use of a container such as SimpleInjector) or you run the risk of running into a severe performance issue.

###Layouts###
Layouts can be added by including them in the same folder as your template and referencing them at the top though a directive at the top of the template.  The layout will automatically be picked up, compiled and cached for quick reuse on the next call.

A layout and partial template are included below:

<script src="https://gist.github.com/jonleigh/dde18c82cad520d3c2a3.js"></script>

###Sending Emails###
The actual sending of an email is done through the ``IEmailDispatcher`` interface allowing you to either use the built in ``SmtpDispatcher`` (which utilises the built in .NET classes) or write your own, most probably because you would like to use the API of your favourite transactional email gateway such as MailJet, Mandrill etc.  It also handily leads into our next point:

###Unit Testing###
It's fairly trivial to write unit tests for specific emails by mocking out the ``IEmailDispatcher`` interface (here using Moq), creating a model and calling the Create method on our RazorMailer class.  A simple test is outlined below:

<script src="https://gist.github.com/jonleigh/e37c2d2b5c8cc4f60c15.js"></script>

###Development and Testing###
The included [sample application]() makes use of a simple SMTP client for windows called [smtp4dev](http://smtp4dev.codeplex.com/). smtp4dev sits in your tray and collects any emails sent to it on port 25, no matter who they're addressed to.

Another option is to use the test email server in a box solution, [Mailtrap](https://mailtrap.io/). Mailtrap provides you with a test mailbox in the cloud that traps all emails sent to it, preventing them being delivered to the end email address. This is especially handy if you want to interact with emails sent from a test environment whilst simultaneously wanting avoiding those embarrassing moments when a user(s) receives an email they shouldn't have.


