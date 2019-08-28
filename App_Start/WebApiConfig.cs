using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Selectcon
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {


            // config.Routes.MapHttpRoute(
            //    name: "defaultApi",
            //    routeTemplate: "api/{controller}/{key}",
            //    defaults: new
            //    {
            //        key = RouteParameter.Optional
            //    }
            //);

            // config.Routes.MapHttpRoute(
            //    name: "Question",
            //    routeTemplate: "api/{controller}/{action}/{key}",
            //    defaults: new {
            //        key = RouteParameter.Optional
            //    }
            //);

            // config.Routes.MapHttpRoute(
            //    name: "Question2",
            //    routeTemplate: "api/Questions/{action}",
            //    defaults: new
            //    {
            //        key = RouteParameter.Optional
            //    }
            //);

        }
    }
}