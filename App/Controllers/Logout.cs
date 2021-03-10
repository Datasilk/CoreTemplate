using Microsoft.AspNetCore.Http;

namespace CoreTemplate.Controllers
{
    public class Logout : Controller
    {
        public override string Render(string body = "")
        {
            User.LogOut();
            return Redirect("/login");
        }
    }
}
