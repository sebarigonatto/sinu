using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SINU.Startup))]
namespace SINU
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
