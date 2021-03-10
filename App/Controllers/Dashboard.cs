using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CoreTemplate.Controllers
{
    public class Dashboard: Controller
    {
        public struct structMenuItem
        {
            public string label;
            public string id;
            public string href;
            public string icon;
            public List<structMenuItem> submenu;
        }

        public override string Render(string body = "")
        {
            //check security
            if (!CheckSecurity()) { return AccessDenied<Login>(); }

            //set up client-side dependencies
            AddCSS("/css/views/dashboard/dashboard.css");
            AddScript("js/views/dashboard/dashboard.js");

            //load the dashboard layout
            var view = new View("/Views/Dashboard/dashboard.html");
            var scaffMenu = new View("/Views/Dashboard/menu-item.html");

            //load user profile
            view["profile-img"] = "";
            view["btn-edit-img"] = "";
            view["profile-name"] = User.DisplayName;

            //load website info
            view["website-name"] = Title;
            view["website-url"] = "http://coretemplate.datasilk.io";
            view["website-url-name"] = "coretemplate.datasilk.io";

            //generate menu system
            var menu = new StringBuilder();
            var menus = new List<structMenuItem>()
            {
                menuItem("Timeline", "timeline", "/dashboard/timeline", "timeline")
            };

            //render menu system
            foreach (var item in menus)
            {
                menu.Append(renderMenuItem(scaffMenu, item, 0));
            }
            view["menu"] = "<ul class=\"menu\">" + menu.ToString() + "</ul>";

            //get dashboard section name
            var subPath = Context.Request.Path.ToString().Replace("dashboard", "").Substring(1);
            if(subPath == "" || subPath == "/") { subPath = "timeline"; }
            var html = "";

            //load dashboard section
            Controller subpage = null;
            var t = LoadSubPage(subPath);
            subpage = t.Item1;
            html = t.Item2;
            if (html == "") { return AccessDenied<Login>(); }
            view["body"] = html;

            //set up page info
            Title = Title + " - Dashboard - " + subpage.Title;

            //include dashboard section javascript dependencies
            Scripts.Append(subpage.Scripts);

            //render base layout along with dashboard section
            return base.Render(view.Render());
        }

        private Tuple<Controller, string> LoadSubPage(string path)
        {
            //get correct sub page from path
            Controller service = null;
            var html = "";
            var paths = path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var subpath = paths.Skip(1).ToArray();

            if (paths[0] == "timeline")
            {
                service = new DashboardPages.Timeline();
                service.Context = Context;
            }
            
            //render sub page
            html = service.Render();

            return new Tuple<Controller, string>(service, html);
        }

        private structMenuItem menuItem(string label, string id, string href, string icon, List<structMenuItem> submenu = null)
        {
            var menu = new structMenuItem();
            menu.label = label;
            menu.id = id;
            menu.href = href;
            menu.icon = icon;
            menu.submenu = submenu;
            return menu;
        }

        private string renderMenuItem(View scaff, structMenuItem item, int level = 0)
        {
            var gutter = "";
            var subs = new StringBuilder();
            for (var x = 0; x < level; x++)
            {
                gutter += "<div class=\"gutter\"></div>";
            }
            if (item.submenu != null)
            {
                if(item.submenu.Count > 0)
                {
                    foreach(var sub in item.submenu)
                    {
                        subs.Append(renderMenuItem(scaff, sub, level + 1));
                    }
                }
            }
            scaff["label"] = item.label;
            scaff["href"] = item.href == "" ? "javascript:" : item.href;
            scaff["section-name"] = item.id;
            scaff["icon"] = item.icon;
            scaff["gutter"] = gutter;
            if(subs.Length > 0)
            {
                scaff["target"] = " target=\"_self\"";
                scaff["submenu"] = "<div class=\"row submenu\"><ul class=\"menu\">" + subs.ToString() + "</ul></div>";
            }
            else
            {
                scaff["submenu"] = "";
            }
            
            return scaff.Render();
        }
    }
}
