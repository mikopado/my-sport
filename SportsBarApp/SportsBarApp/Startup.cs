using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SportsBarApp.Startup))]
namespace SportsBarApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
