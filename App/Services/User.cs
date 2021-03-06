﻿using System;
using System.Net.Mail;
using CoreTemplate.Extensions.Strings;

namespace CoreTemplate.Services
{
    public class User : Service
    {
        public string homePath = "home"; //user home path used to redirect after user log in success

        public string Authenticate(string email, string password)
        {
            var encrypted = Query.Users.GetPassword(email);
            if (encrypted == null) { return Error(); }
            if (!DecryptPassword(email, password, encrypted)) { return Error(); }
            {
                //password verified by Bcrypt
                var user = Query.Users.Authenticate(email, encrypted);
                if (user != null)
                {
                    User.LogIn(user.userId, user.email, user.name, user.datecreated, user.photo);
                    User.Save(true);
                    return JsonResponse(new { redirect = homePath });
                }
            }
            return Error();
        }

        public string CreateAdminAccount(string name, string email, string password, string password2)
        {
            if (App.HasAdmin == true) { return Error(); }
            if (!CheckEmailAddress(email)) { return Error("Email address is invalid"); }
            if (password != password2) { return Error("Passwords do not match"); }
            try
            {
                CheckPassword(password);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
            if (string.IsNullOrEmpty(name)) { return Error("Please specify your name"); }
            Query.Users.CreateUser(new Query.Models.User()
            {
                name = name,
                email = email,
                password = EncryptPassword(email, password)
            }, true);
            App.HasAdmin = true;
            return "success";
        }

        public string SignUp(string name, string emailaddr, string password, string password2)
        {
            if (!CheckEmailAddress(emailaddr)) { return Error("Email address is invalid"); }
            if (Query.Users.Exists(emailaddr)) { return Error("Another account is already using the email address \"" + emailaddr + "\""); }
            if (password != password2) { return Error("Passwords do not match"); }
            try
            {
                CheckPassword(password);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
            if (string.IsNullOrEmpty(name)) { return Error("Please specify your name"); }
            var activationkey = Generate.NewId(16);
            var userId = Query.Users.CreateUser(new Query.Models.User()
            {
                name = name,
                email = emailaddr,
                password = EncryptPassword(emailaddr, password),
                tempkey = activationkey
            });

            //send signup activation email
            var viewEmail = new View("/Content/emails/signup.html");
            viewEmail["userId"] = userId.ToString();
            viewEmail["name"] = name;
            viewEmail["email"] = emailaddr;
            viewEmail["activation-key"] = activationkey;
            //var msg = Core.Email.Create(new MailAddress(emailaddr, name), "Welcome to Saber!", viewEmail.Render());
            //Core.Email.Send(msg, "signup");

            return "success";
        }

        public string RequestActivation(string emailaddr)
        {
            if (Query.Users.CanActivate(emailaddr))
            {
                //var activationkey = Generate.NewId(16);
                //Query.Users.RequestActivation(emailaddr, activationkey);
                //TODO: send signup activation email
            }
            return Success();
        }

        public string Activate(string emailaddr, string activationkey)
        {
            if (Query.Users.Activate(emailaddr, activationkey))
            {
                return Success();
            }
            else
            {
                return Error();
            }
        }

        public string ForgotPassword(string emailaddr)
        {
            if (!Query.Users.CanActivate(emailaddr))
            {
                //var activationkey = Generate.NewId(16);
                //Query.Users.ForgotPassword(emailaddr, activationkey);
                //Query.Logs.LogForgotPassword(emailaddr);
                var viewEmail = new View("/Content/emails/forgot-pass.html");
                viewEmail["email"] = emailaddr;
                //viewEmail["url"] = App.Host + "/resetpass#" + activationkey;
                //viewEmail["activation-key"] = activationkey;
                //var msg = Core.Email.Create(new MailAddress(emailaddr), "Password reset for Saber", viewEmail.Render());
                //Core.Email.Send(msg, "forgotpass");
                return Success();
            }
            return Error("Email is not eligible for a password reset");
        }

        public string ResetPassword(string key, string password, string password2)
        {
            if (password != password2) { return Error("Passwords do not match"); }
            try
            {
                CheckPassword(password);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
            var email = Query.Users.GetEmailFromResetKey(key);
            if (string.IsNullOrEmpty(email))
            {
                return Error("Password reset authentication key has expired.");
            }
            if (Query.Users.ResetPassword(EncryptPassword(email, password), key))
            {
                return Success();
            }
            else
            {
                return Error("Password reset authentication key expired.");
            }
        }

        public void LogOut()
        {
            User.LogOut();
        }

        private bool CheckEmailAddress(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void CheckPassword(string password)
        {
            if (password.Length < 8)
            {
                throw new Exception("Password must be at least " + 8 + " characters in length");
            }
            if (password.Length > 16)
            {
                throw new Exception("Password must be less than " + 17 + " characters in length");
            }
            char lastchar = char.MaxValue;
            var numbers = 0;
            var uppercase = 0;
            var special = 0;
            var spaces = 0;
            var consecutives = 0;
            var maxconsecutives = 0;
            for (var x = 0; x < password.Length; x++)
            {
                var p = password[x];
                if (lastchar == p)
                {
                    consecutives++;
                    if (consecutives > maxconsecutives)
                    {
                        maxconsecutives = consecutives;
                    }
                }
                else if (consecutives > 0)
                {
                    consecutives = 0;
                }
                if (char.IsNumber(p)) { numbers++; }
                else if (char.IsUpper(p)) { uppercase++; }
                else if (p == ' ') { spaces++; }
                else if (char.IsLetter(p)) { }
                else { special++; }
            }

            if (numbers < 1)
            {
                throw new Exception("Password must contain at least 1 number");
            }
            if (numbers < 1)
            {
                throw new Exception("Password must contain at least 1 uppercase letter");
            }
        }

        private string EncryptPassword(string email, string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(email + App.Salt + password, App.BcryptWorkfactor);

        }

        private bool DecryptPassword(string email, string password, string encrypted)
        {
            return BCrypt.Net.BCrypt.Verify(email + App.Salt + password, encrypted);
        }
    }
}