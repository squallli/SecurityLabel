using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(certify.Startup))]
namespace certify
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
