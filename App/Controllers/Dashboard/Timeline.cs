using Microsoft.AspNetCore.Http;

namespace CoreTemplate.Controllers.DashboardPages
{
    public class Timeline: Controller
    {
        public Timeline(HttpContext context, Parameters parameters) : base(context, parameters)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            //load timeline
            var scaffold = new Scaffold("/Views/Dashboard/Timeline/timeline.html");
            return scaffold.Render();
        }
    }
}
