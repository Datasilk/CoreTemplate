namespace CoreTemplate.Controllers
{
    public class Home : Controller
    {
        public override string Render(string body = "")
        {
            var view = new View("/Views/Home/home.html");
            
            if(User.UserId > 0)
            {
                view.Show("user");
                view["username"] = User.Name;
            }
            else
            {
                view.Show("no-user");
            }

            //load header since it was included in home.html
            LoadHeader(ref view);

            //add CSS file for home
            AddCSS("/css/views/home/home.css");

            //finally, render base page layout with home page
            return base.Render(view.Render());
        }
    }
}
