using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyJavaScript.Startup))]
namespace MyJavaScript
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
