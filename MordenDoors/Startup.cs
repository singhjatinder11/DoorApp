using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MordenDoors.Startup))]
namespace MordenDoors
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
