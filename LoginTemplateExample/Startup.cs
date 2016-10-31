using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LoginTemplateExample.Startup))]
namespace LoginTemplateExample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
