using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PortablePhotoGallery.Web.Startup))]
namespace PortablePhotoGallery.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
