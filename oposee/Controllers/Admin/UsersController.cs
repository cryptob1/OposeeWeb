using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using oposee.Models;
using oposee.Models.Models;
using OposeeLibrary.Utilities;
using PagedList;
namespace oposee.Controllers.Admin
{
    public class UsersController : Controller
    {
        private oposeeDbEntities db = new oposeeDbEntities();

        // GET: Users
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "firstname_desc" : "";
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "lastname_desc" : "";
            ViewBag.UserNameSortParm = String.IsNullOrEmpty(sortOrder) ? "username_desc" : "";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewBag.RecordStatusSortParm = String.IsNullOrEmpty(sortOrder) ? "recordstatus_desc" : "";
            ViewBag.DateSortParm = sortOrder == "date" ? "date_desc" : "date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = from s in db.Users
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "username":
                    students = students.OrderBy(s => s.UserName);
                    break;
                case "username_desc":
                    students = students.OrderByDescending(s => s.UserName);
                    break;

                case "firstname_desc":
                    students = students.OrderByDescending(s => s.FirstName);
                    break;
                case "email":
                    students = students.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    students = students.OrderByDescending(s => s.Email);
                    break;
                case "lastname":
                    students = students.OrderBy(s => s.LastName);
                    break;
                case "lastname_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "recordstatus":
                    students = students.OrderBy(s => s.RecordStatus);
                    break;
                case "recordstatus_desc":
                    students = students.OrderByDescending(s => s.RecordStatus);
                    break;
                case "date":
                    students = students.OrderBy(s => s.CreatedDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.CreatedDate);
                    break;
                default:  // Name ascending 
                    students = students.OrderBy(s => s.FirstName);
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
            // return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViewModelUser ViewUser)
        {
            User user = new User();
            if (ModelState.IsValid)
            {
                user.FirstName = ViewUser.FirstName;
                user.LastName = ViewUser.LastName;
                user.UserName = ViewUser.UserName;
                if (!string.IsNullOrEmpty(ViewUser.Password))
                {
                    user.Password = AesCryptography.Encrypt(ViewUser.Password);
                }
                user.Email = ViewUser.Email;
                string _SiteURL = WebConfigurationManager.AppSettings["SiteImgURL"];
                user.ImageURL = _SiteURL + "/ProfileImage/" + ViewUser.ImageURL_data.FileName;
                var path = Path.Combine(Server.MapPath("~/Content/Upload/ProfileImage"), ViewUser.ImageURL_data.FileName);
                ViewUser.ImageURL_data.SaveAs(path);
                user.RecordStatus = "Active";
                user.CreatedDate = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewModelUser modelUser = new ViewModelUser();
            User user = db.Users.Find(id);
            modelUser.UserID = user.UserID;
            modelUser.SocialID = user.SocialID;
            modelUser.SocialType = user.SocialType;
            modelUser.DeviceToken = user.DeviceToken;
            modelUser.DeviceType = user.DeviceType;
            modelUser.RecordStatus = user.RecordStatus;
            modelUser.CreatedDate = user.CreatedDate;
            modelUser.FirstName = user.FirstName;
            modelUser.LastName = user.LastName;
            modelUser.UserName = user.UserName;
            modelUser.CreatedDate = user.CreatedDate;
            if (!string.IsNullOrEmpty(user.Password))
            {
                modelUser.Password = AesCryptography.Decrypt(user.Password);
            }
            modelUser.Email = user.Email;
            string _SiteURL = WebConfigurationManager.AppSettings["SiteImgURL"];
            modelUser.ImageURL = user.ImageURL;
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(modelUser);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ViewModelUser ViewUser)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                user.FirstName = ViewUser.FirstName;
                user.LastName = ViewUser.LastName;
                user.UserName = ViewUser.UserName;
                if (!string.IsNullOrEmpty(ViewUser.Password))
                {
                    user.Password = AesCryptography.Encrypt(ViewUser.Password);
                }
                user.Email = ViewUser.Email;
                string _SiteURL = WebConfigurationManager.AppSettings["SiteImgURL"];
                //user.ImageURL = _SiteURL + "/ProfileImage/" + ViewUser.ImageURL_data.FileName;
                //var path = Path.Combine(Server.MapPath("~/Content/Upload/ProfileImage"), ViewUser.ImageURL_data.FileName);
                //ViewUser.ImageURL_data.SaveAs(path);
                user.CreatedDate = ViewUser.CreatedDate;
                user.ImageURL = ViewUser.ImageURL;
                user.UserID = ViewUser.UserID;
                user.DeviceType = ViewUser.DeviceType;
                user.DeviceToken = ViewUser.DeviceToken;
                user.SocialID = ViewUser.SocialID;
                user.SocialType = ViewUser.SocialType;
                user.RecordStatus = "Active";
                user.ModifiedDate = DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ViewUser);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
