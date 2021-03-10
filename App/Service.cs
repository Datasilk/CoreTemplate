using System.Linq;

namespace CoreTemplate
{
    public class Service : Datasilk.Core.Web.Service
    {

        private User user;

        public User User
        {
            get
            {
                if (user == null)
                {
                    user = new User();
                }
                return user;
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