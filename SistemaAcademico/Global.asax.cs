using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace SistemaAcademico
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            /* System.Data.Entity.Database.SetInitializer(new SistemaAcademico.Data.DatabaseInitializer());

             using (var context = new ApplicationDbContext())
             {
                 var initializer = new SistemaAcademico.Data.DatabaseInitializer();
                 initializer.EjecutarSeed(context);
             }*/

        }
    }
}
