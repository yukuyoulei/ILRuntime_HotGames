using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        config.MapHttpAttributeRoutes();

        config.Routes.MapHttpRoute(
            "DefaultApi",
            "api/{controller}/{action}",
            new { id = RouteParameter.Optional }
        );
    }
}