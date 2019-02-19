using OposeeLibrary.Utilities;
using oposee.Models;
using oposee.Models.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace oposee.Controllers
{
    public class UserController : Controller
    {
        //Registration Action
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        //Registration POST action 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] User user)
        {
            bool Status = false;
            string message = "";
            //
            // Model Validation 
            if (ModelState.IsValid)
            {

                #region //Email is already Exist 
                var isExist = IsEmailExist(user.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                #endregion

                #region Generate Activation Code 
               // user.ActivationCode = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
                user.Password = Crypto.Hash(user.Password);
               // user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword); //
                #endregion
                //user.IsEmailVerified = false;

                #region Save to Database
                using (oposeeDbEntities dc = new oposeeDbEntities())
                {
                    dc.Users.Add(user);
                    dc.SaveChanges();

                    //Send Email to User
                   // SendVerificationLinkEmail(user.Email, user.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link " +
                        " has been sent to your email id:" + user.Email;
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }
        //Verify Account  

        //[HttpGet]
        //public ActionResult VerifyAccount(string id)
        //{
        //    bool Status = false;
        //    using (oposeeDbEntities db = new oposeeDbEntities())
        //    {
        //        db.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
        //                                                        // Confirm password does not match issue on save changes
        //        var v = db.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
        //        if (v != null)
        //        {
        //            v.IsEmailVerified = true;
        //            db.SaveChanges();
        //            Status = true;
        //        }
        //        else
        //        {
        //            ViewBag.Message = "Invalid Request";
        //        }
        //    }
        //    ViewBag.Status = Status;
        //    return View();
        //}

        //Login 
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            string message = "";
            using (oposeeDbEntities db = new oposeeDbEntities())
            {
                var v = db.Users.Where(a => a.Email == login.EmailID).FirstOrDefault();
                if (v != null)
                {
                    //if (!v.IsEmailVerified)
                    //{
                    //    ViewBag.Message = "Please verify your email first";
                    //    return View();
                    //}

                    if (string.Compare(AesCryptography.Encrypt(login.Password), v.Password) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);


                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        //Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }


        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (oposeeDbEntities db = new oposeeDbEntities())
            {
                var v = db.Users.Where(a => a.Email == emailID).FirstOrDefault();
                return v != null;
            }
        }

        //[NonAction]
        //public void SendVerificationLinkEmail(string emailID, string activationCode)
        //{
        //    var verifyUrl = "/User/VerifyAccount/" + activationCode;
        //    var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

        //    var fromEmail = new MailAddress("dotnetawesome@gmail.com", "Dotnet Awesome");
        //    var toEmail = new MailAddress(emailID);
        //    var fromEmailPassword = "********"; // Replace with actual password
        //    string subject = "Your account is successfully created!";

        //    string body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
        //        " successfully created. Please click on the below link to verify your account" +
        //        " <br/><br/><a href='" + link + "'>" + link + "</a> ";

        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com",
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
        //    };

        //    using (var message = new MailMessage(fromEmail, toEmail)
        //    {
        //        Subject = subject,
        //        Body = body,
        //        IsBodyHtml = true
        //    })
        //        smtp.Send(message);
        //}
    }
}