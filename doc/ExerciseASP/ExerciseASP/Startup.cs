using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ExerciseASP.Startup))]
namespace ExerciseASP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
