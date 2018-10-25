using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sofiapi.net.Models
{
    public class ApiRoute
    {
        public int RouteID { get; set; }

        public string AppName { get; set; }

        public string RoutePath { get; set; }

        public string RouteCommand { get; set; }

        public bool AllowNoParameters { get; set; }

        public bool PublicRoute { get; set; }

        public string PermissionList { get; set; }

        public DateTime CreateDt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }

        public string ModifiedBy { get; set; }


        public List<ProcedureParameter> parameters { get; set; }
    }
}