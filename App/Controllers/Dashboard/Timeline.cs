namespace CoreTemplate.Controllers.DashboardPages
{
    public class Timeline: Controller
    {
        public override string Render(string body = "")
        {
            //load timeline
            var view = new View("/Views/Dashboard/Timeline/timeline.html");
            return view.Render();
        }
    }
}
