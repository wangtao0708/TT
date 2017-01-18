using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(tt2.Wx.Web2.Startup))]
namespace tt2.Wx.Web2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
