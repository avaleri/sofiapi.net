using sofiapi.net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sofiapi.net.Interfaces
{
    public interface IDB
    {
        ApiRoute GetRouteByPath(string path);

        string ExecuteRoute(ApiRoute route);
    }
}