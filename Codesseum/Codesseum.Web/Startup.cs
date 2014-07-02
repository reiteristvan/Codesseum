using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Codesseum.Web.Startup))]
namespace Codesseum.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
