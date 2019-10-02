using Microsoft.AspNetCore.Http;

namespace CoreTemplate
{
    public class Controller : Datasilk.Mvc.Controller
    {
        public Controller(HttpContext context, Parameters parameters) : base(context, parameters)
        {
            title = "CoreTemplate";
            description = "You can do everything you ever wanted";
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            scripts.Append("<script language=\"javascript\">S.svg.load('/images/icons.svg');</script>");
            return base.Render(path, body, metadata);
        }

        public void LoadHeader(ref Scaffold scaffold)
        {
            if(User.userId > 0)
            {
                scaffold.Child("header").Show("user");
            }
            else
            {
                scaffold.Child("header").Show("no-user");
            }
        }
    }
}