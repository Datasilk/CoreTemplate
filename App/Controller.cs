using System.Linq;

namespace CoreTemplate
{
    public class Controller : Datasilk.Core.Web.Controller
    {
        public string Title { get; set; }
        public string Description { get; set; }

        private User user;

        public User User { get
            {
                if(user == null)
                {
                    user = new User();
                }
                return user;
            } 
        }

        public override string Render(string body = "")
        {
            Title = "CoreTemplate";
            Description = "You can do everything you ever wanted";
            Scripts.Append("<script language=\"javascript\">S.svg.load('/images/icons.svg');</script>");
            return base.Render(body);
        }

        public void LoadHeader(ref View view)
        {
            if(User.UserId > 0)
            {
                view.Child("header").Show("user");
            }
            else
            {
                view.Child("header").Show("no-user");
            }
        }

        public bool CheckSecurity(string key = "")
        {
            if (User.UserId == 1) { return true; }
            if (key != "" && User.UserId > 0 && !User.Keys.Any(a => a.Key == key && a.Value == true))
            {
                return false;
            }
            else if (key == "" && User.UserId <= 0)
            {
                return false;
            }
            return true;
        }
    }
}