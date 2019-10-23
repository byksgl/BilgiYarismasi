using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BilgiYarismasi.Web.Startup))]
namespace BilgiYarismasi.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
