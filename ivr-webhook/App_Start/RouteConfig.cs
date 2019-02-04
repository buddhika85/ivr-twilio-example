using System.Web.Mvc;
using System.Web.Routing;

namespace ivr_webhook
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Ivr", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
