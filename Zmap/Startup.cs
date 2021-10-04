using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Zmap.Startup))]
namespace Zmap
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
