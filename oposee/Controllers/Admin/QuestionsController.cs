using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using oposee.Models.API;
using oposee.Models.Models;
using PagedList;
namespace oposee.Controllers.Admin
{
    public class QuestionsController : Controller
    {
        private oposeeDbEntities db = new oposeeDbEntities();

        // GET: Questions
        public ActionResult Index(int? userId, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.QuestionSortParm = String.IsNullOrEmpty(sortOrder) ? "question_desc" : "";
            ViewBag.HashSortParm = String.IsNullOrEmpty(sortOrder) ? "hashtags_desc" : "";
            ViewBag.UserNameSortParm = String.IsNullOrEmpty(sortOrder) ? "user_desc" : "";
            ViewBag.UserId = userId;
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
            var questList = new List<PostQuestionDetail>();
            if (userId != null)
            {
                questList = (from q in db.Questions
                             join u in db.Users on q.OwnerUserID equals u.UserID
                             where u.UserID == userId
                             select new PostQuestionDetail
                             {
                                 Id = q.Id,
                                 Question = q.PostQuestion,
                                 OwnerUserID = q.OwnerUserID,
                                 OwnerUserName = u.UserName,
                                 //UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                 HashTags = q.HashTags,
                                 CreationDate = q.CreationDate
                             }).ToList();
            }
            else
            {
                questList = (from q in db.Questions
                             join u in db.Users on q.OwnerUserID equals u.UserID
                             select new PostQuestionDetail
                             {
                                 Id = q.Id,
                                 Question = q.PostQuestion,
                                 OwnerUserID = q.OwnerUserID,
                                 OwnerUserName = u.UserName,
                                 //UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                 HashTags = q.HashTags,
                                 CreationDate = q.CreationDate
                             }).ToList();
            }
            
            if (!String.IsNullOrEmpty(searchString))
            {
                questList = questList.Where(s => s.Question.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
               
                case "question_desc":
                    questList = questList.OrderByDescending(s => s.Question).ToList();
                    break;
                case "hashtags_desc":
                    questList = questList.OrderByDescending(s => s.HashTags).ToList();
                    break;
                case "hashtags":
                    questList = questList.OrderBy(s => s.HashTags).ToList(); 
                    break;
                case "user_desc":
                    questList = questList.OrderByDescending(s => s.OwnerUserName).ToList();
                    break;
                case "user":
                    questList = questList.OrderBy(s => s.OwnerUserName).ToList();
                    break;
                case "date":
                    questList = questList.OrderBy(s => s.CreationDate).ToList();
                    break;
                case "date_desc":
                    questList = questList.OrderByDescending(s => s.CreationDate).ToList();
                    break;
                default:  // Name ascending 
                    questList = questList.OrderBy(s => s.Question).ToList();
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(questList.ToPagedList(pageNumber, pageSize));
            //return View(questList);
        }


        // GET: Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Questions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PostQuestion,OwnerUserID,HashTags,CreationDate,ModifiedDate")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(question);
        }

        // GET: Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PostQuestion,OwnerUserID,HashTags,CreationDate,ModifiedDate")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
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
