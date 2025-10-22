using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SistemaAcademico.Startup))]
namespace SistemaAcademico
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
