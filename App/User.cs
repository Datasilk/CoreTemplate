﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CoreTemplate
{
    public class User
    {
        private bool changed = false;
        private HttpContext Context;

        public int UserId { get; set; } = 0;
        public short UserType { get; set; } = 0;
        public string Email { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Photo { get; set; }
        public DateTime DateCreated { get; set; }
        public string Language { get; set; }
        public List<KeyValuePair<string, bool>> Keys { get; set; } = new List<KeyValuePair<string, bool>>();
        public int[] Groups { get; set; } = new int[] { };
        public bool ResetPass { get; set; }

        public static User Get(HttpContext Context)
        {
            User user;
            if (Context.Session.Get("user") != null)
            {
                user = JsonSerializer.Deserialize<User>(GetString(Context.Session.Get("user")));
            }
            else
            {
                user = (User)new User().SetContext(Context);
            }
            user.Init(Context);
            return user;
        }

        public User SetContext(HttpContext Context)
        {
            Context = Context;
            return this;
        }

        public void Init(HttpContext Context)
        {
            //generate visitor id
            Context = Context;

            //check for persistant cookie
            if (UserId <= 0 && Context.Request.Cookies.ContainsKey("authId"))
            {
                var user = Query.Users.Authenticate(Context.Request.Cookies["authId"]);
                if (user != null)
                {
                    //persistant cookie was valid, log in
                    LogIn(user.userId, user.email, user.name, user.datecreated, user.photo);
                }
            }
        }

        public void Save(bool changed = false)
        {
            if (this.changed == true && changed == false)
            {
                Context.Session.Set("user", GetBytes(JsonSerializer.Serialize(this)));
                this.changed = false;
            }
            if (changed == true)
            {
                this.changed = true;
            }
        }

        public void LogIn(int userId, string email, string name, DateTime datecreated, bool photo = false)
        {
            UserId = userId;
            Email = email;
            Photo = photo;
            Name = name;
            DateCreated = datecreated;

            var keys = Query.Security.Keys.GetByUserId(userId);
            foreach (var key in keys)
            {
                Keys.Add(new KeyValuePair<string, bool>(key.key, key.value));
            }
            var groups = Query.Security.Users.GetGroups(userId);
            if (groups != null && groups.Count > 0)
            {
                Groups = groups.Select(a => a.groupId).ToArray();
            }

            //create persistant cookie
            var auth = Query.Users.CreateAuthToken(UserId);
            var options = new CookieOptions()
            {
                Expires = DateTime.Now.AddMonths(1)
            };

            Context.Response.Cookies.Append("authId", auth, options);

            changed = true;
        }

        public void LogOut()
        {
            UserId = 0;
            Email = "";
            Name = "";
            Photo = false;
            changed = true;
            Context.Response.Cookies.Delete("authId");
            Save();
        }

        public void SetLanguage(string language)
        {
            Language = language;
            changed = true;
        }

        #region "Editor UI"
        public string[] GetOpenTabs()
        {
            //gets a list of open tabs within the Editor UI
            if (Context.Session.Get("open-tabs") != null)
            {
                return JsonSerializer.Deserialize<string[]>(GetString(Context.Session.Get("open-tabs")));
            }
            else
            {
                return new string[] { };
            }
        }

        public void SaveOpenTabs(string[] tabs)
        {
            Context.Session.Set("open-tabs", GetBytes(JsonSerializer.Serialize(tabs)));
        }

        public void AddOpenTab(string filePath)
        {
            var tabs = GetOpenTabs().ToList();
            if (!tabs.Contains(filePath))
            {
                tabs.Add(filePath);
            }
            SaveOpenTabs(tabs.ToArray());
        }

        public void RemoveOpenTab(string filePath)
        {
            var tabs = GetOpenTabs().ToList();
            if (tabs.Contains(filePath))
            {
                tabs.Remove(filePath);
            }
            SaveOpenTabs(tabs.ToArray());
        }
        #endregion

        #region "Helpers"

        protected static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return string.Join("", chars);
        }

        protected static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string NewId(int length = 3)
        {
            string result = "";
            for (var x = 0; x <= length - 1; x++)
            {
                int type = new Random().Next(1, 3);
                int num;
                switch (type)
                {
                    case 1: //a-z
                        num = new Random().Next(0, 26);
                        result += (char)('a' + num);
                        break;

                    case 2: //A-Z
                        num = new Random().Next(0, 26);
                        result += (char)('A' + num);
                        break;

                    case 3: //0-9
                        num = new Random().Next(0, 9);
                        result += (char)('1' + num);
                        break;

                }

            }
            return result;
        }

        #endregion
    }
}
