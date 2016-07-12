using System.Threading.Tasks;
using System.Web.Mvc;
using Sample.Core.Email.Models;
using Sample.Core.Email;

namespace Sample.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly Mailer _mailer;

        public HomeController(Mailer mailer)
        {
            _mailer = mailer;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SendWelcomeEmail(WelcomeModel model)
        {
            await _mailer.SendWelcomeEmailAsync(model);

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> SendSimpleWelcomeEmail(WelcomeModel model)
        {
            await _mailer.SendSimpleWelcomeEmailAsync(model);

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> SendCatEmail(WelcomeModel model)
        {
            await _mailer.SendCatEmailAsync(model);

            return View("Index", model);
        }
    }
}