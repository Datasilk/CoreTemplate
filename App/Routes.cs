using System;
using Microsoft.AspNetCore.Http;
using Datasilk.Core.Web;

public class Routes : Datasilk.Core.Web.Routes
{
    public override IController FromControllerRoutes(HttpContext Context, Parameters parameters, string name)
    {
        switch (name)
        {
            case "": case "home": return new CoreTemplate.Controllers.Home();
            case "login": return new CoreTemplate.Controllers.Login();
        }
        return null;
    }
}
