using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sample.Web.Startup))]
namespace Sample.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
