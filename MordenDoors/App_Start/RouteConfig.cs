using System.Web.Mvc;
using System.Web.Routing;

namespace MordenDoors
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                   "Quotes",
                   "Quotes/{action}/{id}",
                   defaults: new
                   {
                       controller = "Order",
                       action = "Index",
                       id = UrlParameter.Optional
                   }
                );
            routes.MapRoute(
                   "Workshop",
                   "Workshop/{action}/{id}",
                   defaults: new
                   {
                       controller = "Order",
                       action = "Index",
                       id = UrlParameter.Optional
                   }
                );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
        }
    }
}
