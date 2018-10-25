using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using static sofiapi.net.Services.sofiapi;

namespace sofiapi.net
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ApiRoute",
                url: "{*permalink}",
                defaults: new { controller = "Api", action = "Index" },
                constraints: new { permalink = new ApiUrlConstraint() }
            );
            // Adapted from dynamic routes for database https://stackoverflow.com/questions/16026441/dynamic-routes-from-database-for-asp-net-mvc-cms
            // (shakib answer)

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
