using Newtonsoft.Json;
using oposee.Models;
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
            var data = (from q in db.Questions
                        join o in db.Opinions on q.Id equals o.QuestId
                        select new
                        {
                            Id = o.QuestId
                        }).Distinct();
            DashBoard dashboard = new DashBoard();

            dashboard.UserCount = db.Users.Where(p => p.IsAdmin == false).Count();
            dashboard.QuestionCount = db.Questions.Count();
            dashboard.AnsweredQuestion = data.Count();
            dashboard.UnAnsweredQues = (db.Questions.Count() - data.Count());


            //JsonSerializerSettings _jsonSetting = new JsonSerializerSettings();
            //    var query = (from q in db.Questions
            //                 group q by q.CreationDate.Value.Month into g
            //                 select new
            //                 {
            //                     Month = g.Key,
            //                     Count = g.Count()
            //                 }).ToList();
            //ViewBag.DataPoints = JsonConvert.SerializeObject(query.ToList(), _jsonSetting);
            List<Point> dataPoints = new List<Point>{
                new Point(10, 22),
                new Point(20, 36),
                new Point(30, 42),
                new Point(40, 51),
                new Point(50, 46),
            };

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            //JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            return View(dashboard);
        }
        //private void GetQuesMonth(int month)  // this may be a string, change accordingly
        //{
        //    switch (month)
        //    {
        //        case 1:
        //            month = "Jan";
        //            break;
        //        case 2:
        //            month = "Feb";
        //            break;
        //            // and so on
        //    }
        //    return month;
        //}
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
