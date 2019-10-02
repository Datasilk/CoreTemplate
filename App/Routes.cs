using Microsoft.AspNetCore.Http;

public class Routes : Datasilk.Web.Routes
{
    public override Datasilk.Mvc.Controller FromControllerRoutes(HttpContext context, Parameters parameters, string name)
    {
        switch (name)
        {
            case "": case "home": return new CoreTemplate.Controllers.Home(context, parameters);
            case "login": return new CoreTemplate.Controllers.Login(context, parameters);
        }
        return null;
    }
}
