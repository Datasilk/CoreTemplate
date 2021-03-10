using System.Linq;

namespace CoreTemplate
{
    public class Controller : Datasilk.Core.Web.Controller
    {
        public string Title { get; set; } = "Datasilk Core Template";
        public string Description { get; set; } = "You can do everything you ever wanted";
        public string Theme { get; set; } = "default";

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
            Scripts.Append("<script language=\"javascript\">S.svg.load('/images/icons.svg');</script>");
            var view = new View("/Views/Shared/layout.html");
            //add head meta data
            view["title"] = Title;
            view["description"] = Description;
            //set CSS theme
            view["theme"] = Theme;
            //set user language
            view["language"] = User.Language;
            //add page content
            view["body"] = body;
            //add any CSS stylesheets
            view["head-css"] = Css.ToString();
            //add any JS script files
            view["scripts"] = Scripts.ToString();

            return view.Render();
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