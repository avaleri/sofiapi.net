using sofiapi.net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sofiapi.net.Controllers
{
    public class ApiController : Controller
    {

        public bool Authenticate(ApiRoute route)
        {
            var result = false;
            if (!route.PublicRoute && !String.IsNullOrWhiteSpace(route.PermissionList))
            {
                if (!Request.IsAuthenticated)
                {
                    // access denied.
                    return false;
                }

                if (User.Identity == null || String.IsNullOrWhiteSpace(User.Identity.Name))
                {
                    // access denied.
                    return false;
                }

                List<string> allowedRoles = route.PermissionList.Split(',').ToList();
                foreach (var role in allowedRoles)
                {
                    if (User.IsInRole(role))
                    {
                        result = true;
                        break;
                    }
                }
            }
            else
            {
                result = true;
                // route is public, or no permissions specified.
            }
            return result;
        }

        // GET: Api
        public ActionResult Index()
        {
            var route = HttpContext.Items["route"] as ApiRoute;
            //show the content with view
            sofiapi.net.Services.sofiapi sp = new Services.sofiapi();

            if(Authenticate(route))
            {
                var result = sp.ExecuteRoute(route, Request);
                return new ContentResult() { ContentType = "application/json", Content = result };
            }
            else
            {
                return new HttpStatusCodeResult(401);
            }

        }
    }
}