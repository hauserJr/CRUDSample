using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SearchRelease.Startup))]
namespace SearchRelease
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
