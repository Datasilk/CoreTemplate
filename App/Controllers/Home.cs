using Microsoft.AspNetCore.Http;

namespace CoreTemplate.Controllers
{
    public class Home : Controller
    {
        public Home(HttpContext context, Parameters parameters) : base(context, parameters)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            var scaffold = new Scaffold("/Views/Home/home.html");
            
            if(User.userId > 0)
            {
                scaffold.Show("user");
                scaffold["username"] = User.name;
            }
            else
            {
                scaffold.Show("no-user");
            }

            //load header since it was included in home.html
            LoadHeader(ref scaffold);

            //add CSS file for home
            AddCSS("/css/views/home/home.css");

            //finally, render base page layout with home page
            return base.Render(path, scaffold.Render(), metadata);
        }
    }
}
