using sofiapi.net.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using sofiapi.net.Models;
using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace sofiapi.net.Services
{
    public class sofiapi
    {
        private static IDB db;

        public sofiapi()
        {

        }

        public sofiapi(IDB _db)
        {
            db = _db;
        }

        public List<ProcedureParameter> GetParamsFromQueryString(NameValueCollection nvc, List<ProcedureParameter> parameters)
        {
            List<ProcedureParameter> procParams = new List<ProcedureParameter>();
            foreach (string item in nvc)
            {
                ProcedureParameter matchingParam = parameters.Where(E => E.PARAMETER_NAME == "@" + item).SingleOrDefault();
                if (matchingParam != null)
                {
                    string value = nvc[item];
                    matchingParam.VALUE = value;
                    procParams.Add(matchingParam);
                }
            }
            return procParams;
        }

        public List<ProcedureParameter> GetParamsFromRequestBody(Dictionary<string,string> nvc, List<ProcedureParameter> parameters)
        {
            List<ProcedureParameter> procParams = new List<ProcedureParameter>();
            foreach (string key in nvc.Keys)
            {
                ProcedureParameter matchingParam = parameters.Where(E => E.PARAMETER_NAME == "@" + key).SingleOrDefault();
                if (matchingParam != null)
                {
                    string value = nvc[key];
                    matchingParam.VALUE = value;
                    procParams.Add(matchingParam);
                }
            }
            return procParams;
        }

        public string ExecuteRoute(Models.ApiRoute r, HttpRequestBase Request)
        {
            var result = String.Empty;
            if(Request.QueryString.Count > 0)
            {
                var procParams = GetParamsFromQueryString(Request.QueryString, r.parameters);

                r.parameters = procParams;
                result = db.ExecuteRoute(r);
            }
            else
            {
                Request.InputStream.Seek(0, SeekOrigin.Begin);
                string jsonData = new StreamReader(Request.InputStream).ReadToEnd();
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                var procParams = GetParamsFromRequestBody(values, r.parameters);

                r.parameters = procParams;
                result = db.ExecuteRoute(r);
            }

            return result;
        }

        public class ApiUrlConstraint : IRouteConstraint
        {
            public bool Match(HttpContextBase httpContext, System.Web.Routing.Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
            {
                if (values[parameterName] != null)
                {
                    var permalink = values[parameterName].ToString();
                    if(!permalink.StartsWith("/"))
                    {
                        permalink = "/" + permalink;
                    }

                    var r = db.GetRouteByPath(permalink);
                    if(r != null && !String.IsNullOrWhiteSpace(r.RoutePath))
                    {
                        httpContext.Items["route"] = r;
                        return true;
                    }
                }
                return false;
            }
        }
    }
}