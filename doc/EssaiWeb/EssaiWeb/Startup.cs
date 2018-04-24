using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EssaiWeb.Startup))]
namespace EssaiWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
