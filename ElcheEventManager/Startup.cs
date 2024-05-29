using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ElcheEventManager.Startup))]
namespace ElcheEventManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
