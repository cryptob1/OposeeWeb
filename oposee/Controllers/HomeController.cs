using oposee.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace oposee.Controllers
{
    public class HomeController : Controller
    {
        oposeeDbEntities db = new oposeeDbEntities();
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        [HttpPost]
        public ActionResult Index(User user)
        {

            db.Users.Add(user);
            db.SaveChanges();
            string message = string.Empty;
            switch (user.UserID)
            {
                case -1:
                    message = "Username already exists.\\nPlease choose a different username.";
                    break;
                case -2:
                    message = "Supplied email address has already been used.";
                    break;
                default:
                    message = "Registration successful.\\nUser Id: " + user.UserID.ToString();
                    break;
            }
            ViewBag.Message = message;

            return View(user);
        }
    }
}
