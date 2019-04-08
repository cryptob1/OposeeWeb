using oposee.Enums;
using oposee.Models;
using oposee.Models.API;
using oposee.Models.Models;
using OposeeLibrary.API;
using OposeeLibrary.PushNotfication;
using OposeeLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace oposee.Controllers.API
{
    [RoutePrefix("oposee")]
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
    public class WebApiController : ApiController
    {
        public static string con = ConfigurationManager.ConnectionStrings["oposeeDbEntitiesSp"].ToString();
        oposeeDbEntities db = new oposeeDbEntities();
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }


        [HttpPost]
        [Route("api/WebApi/RegisterUser")]
        public HttpResponseMessage RegisterUser(User users)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }

                User entity = null;
                entity = db.Users.Find(users.UserID);

                string strThumbnailURLfordb = null;
                string strIamgeURLfordb = null;
                string _SiteRoot = WebConfigurationManager.AppSettings["SiteImgPath"];
                string _SiteURL = WebConfigurationManager.AppSettings["SiteImgURL"];

                string strThumbnailImage = users.ImageURL;
                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, entity, "User Exists"));
                }
                else
                {
                    entity = new User();
                    Token token = new Token();
                    entity.UserName = users.FirstName + " " + users.LastName;
                    entity.FirstName = users.FirstName;
                    entity.LastName = users.LastName;
                    entity.Email = users.Email;
                    entity.IsAdmin = false;
                    bool Email = false;
                    Email = OposeeLibrary.Utilities.Helper.IsValidEmail(users.Email);
                    if (!string.IsNullOrEmpty(users.Password))
                    {
                        entity.Password = AesCryptography.Encrypt(users.Password);
                    }

                    entity.DeviceType = "Web";
                    entity.DeviceToken = users.DeviceToken;
                    entity.CreatedDate = DateTime.Now;
                    entity.RecordStatus = RecordStatus.Active.ToString();
                    // entity.SocialID = users.ThirdPartyId;
                    //if (input.ThirdPartyType == ThirdPartyType.Facebook)
                    //{
                    //    entity.SocialType = ThirdPartyType.Facebook.ToString();
                    //}
                    //else if (input.ThirdPartyType == ThirdPartyType.GooglePlus)
                    //{
                    //    entity.SocialType = ThirdPartyType.GooglePlus.ToString();
                    //}
                    //else if (input.ThirdPartyType == ThirdPartyType.Twitter)
                    //{
                    //    entity.SocialType = ThirdPartyType.Twitter.ToString();
                    //}

                    if (users.ImageURL != null && users.ImageURL != "")
                    {
                        try
                        {

                            string strTempImageSave = OposeeLibrary.Utilities.ResizeImage.Download_Image(users.ImageURL);
                            string profileFilePath = _SiteURL + "/ProfileImage/" + strTempImageSave;
                            strIamgeURLfordb = profileFilePath;
                            entity.ImageURL = profileFilePath;
                        }
                        catch (Exception ex)
                        {
                            strThumbnailURLfordb = strThumbnailImage;
                            strIamgeURLfordb = strThumbnailImage;
                        }
                    }
                    else
                    {
                        entity.ImageURL = _SiteURL + "/ProfileImage/oposee-profile.png";
                    }
                    db.Users.Add(entity);
                    db.SaveChanges();

                    int userID = entity.UserID;
                    token.TotalToken = 100;
                    token.BalanceToken = 100;
                    token.UserId = userID;
                    db.Tokens.Add(token);
                    db.SaveChanges();
                    entity = db.Users.Find(userID);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, entity, "UserData"));
                }
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);

                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "UserData"));
            }
        }


        [HttpPost]
        [Route("api/WebApi/Login")]
        public UserLoginWeb Login(UserLoginWeb login)
        {
            UserLoginWeb ObjLogin = new UserLoginWeb();
            using (oposeeDbEntities db = new oposeeDbEntities())
            {
                // UserLogin userlogin = new UserLogin();
                var v1 = db.Users.Select(s => s).ToList();
                var v = db.Users.Where(a => a.Email == login.Email && (a.IsAdmin ?? false) == false).FirstOrDefault();
                if (v != null)
                {
                    ObjLogin.Token = AesCryptography.Encrypt(login.Password);
                    ObjLogin.Token = AesCryptography.Decrypt(ObjLogin.Token);
                    if (string.Compare(AesCryptography.Encrypt(login.Password), v.Password) == 0)
                    {
                        //int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        //var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                        //string encrypted = FormsAuthentication.Encrypt(ticket);
                        //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        //cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        //cookie.HttpOnly = true;
                        //Response.Cookies.Add(cookie);

                        //userlogin.EmailID = login.EmailID;
                        //userlogin.Password = login.Password;
                        login.Id = v.UserID;
                        ObjLogin.Id = v.UserID;
                        ObjLogin.Email = v.Email;
                        ObjLogin.ImageURL = v.ImageURL;


                        return ObjLogin;

                    }
                    else
                    {
                        return ObjLogin;
                    }
                }
                else
                {
                    return ObjLogin;
                }
            }

        }


        [HttpGet]
        [Route("api/WebApi/GetQuestion")]
        public List<PostQuestionDetail> GetQuestion()
        {
            try
            {
                var questList = new List<PostQuestionDetail>();
                questList = (from q in db.Questions
                             join u in db.Users on q.OwnerUserID equals u.UserID
                             select new PostQuestionDetail
                             {
                                 Id = q.Id,
                                 Question = q.PostQuestion,
                                 OwnerUserID = q.OwnerUserID,
                                 OwnerUserName = u.UserName,
                                 HashTags = q.HashTags,
                                 CreationDate = q.CreationDate,
                                 TotalLikes = (from o in db.Opinions
                                               where o.QuestId == q.Id
                                               select o.Likes).Sum(),
                             }).ToList();









                return questList;



            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return null;
            }
        }

        [HttpGet]
        [Route("api/WebApi/GetUserALLRecords")]
        public List<PostQuestionDetailWEB> GetUserALLRecords()
        {
            List<PostQuestionDetailWEB> Objlikdelist = new List<PostQuestionDetailWEB>();
            try
            {
                //var questList = new List<PostQuestionDetail>();
                //ObjPostQuestionDetailList = (from q in db.Questions
                //                             join u in db.Users on q.OwnerUserID equals u.UserID
                //                             join o in db.Opinions on q.Id equals o.QuestId
                //                             join n in db.Notifications on o.Id equals n.CommentId
                //                             select new PostQuestionDetail
                //                             {
                //                                 Id = q.Id,
                //                                 //QuestionId = o.Id,
                //                                 //NotificationId = n.CommentId,
                //                                 Question = q.PostQuestion,
                //                                 OwnerUserID = q.OwnerUserID,
                //                                 OwnerUserName = u.UserName,
                //                                 HashTags = q.HashTags,
                //                                 CreationDate = q.CreationDate
                //                             }).OrderBy(x => x.Id).Take(4).ToList();

                SqlConnection connection = new SqlConnection(con);
                var command1 = new SqlCommand("SP_GetTopLikes", connection);
                command1.CommandType = System.Data.CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader reader = command1.ExecuteReader();



                PostQuestionDetailWEB objitem = null;
                while (reader.Read())
                {
                    objitem = new PostQuestionDetailWEB();
                    objitem.Id = Convert.ToInt32(reader["questid"]);
                    //objitem.Like = Convert.ToInt32(reader["Like"]);
                    objitem.OwnerUserName = reader["UserName"].ToString();
                    objitem.Question = reader["PostQuestion"].ToString();
                    objitem.HashTags = reader["HashTags"].ToString();

                    Objlikdelist.Add(objitem);
                }


                return Objlikdelist;
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return null;
            }
        }


        #region "Post Question" 
        [HttpPost]
        [Route("api/WebApi/PostQuestionWeb")]
        public Token PostQuestionWeb([FromBody] Question postQuestion)
        {
            Token ObjToken = null;
            try
            {

                if (!ModelState.IsValid)
                {
                    return ObjToken; ;
                }
                ObjToken = db.Tokens.Where(x => x.UserId == postQuestion.OwnerUserID).FirstOrDefault();
                if (ObjToken.BalanceToken <= 0)
                {
                    return ObjToken;
                }

                Question quest = null;
                quest = db.Questions.Where(p => p.Id == postQuestion.Id
                                       ).FirstOrDefault();
                //if (quest != null)
                //{
                //    quest.PostQuestion = postQuestion.PostQuestion;
                //    quest.OwnerUserID = postQuestion.OwnerUserID;
                //    quest.HashTags = postQuestion.HashTags;
                //    quest.IsDeleted = false;
                //    quest.ModifiedDate = DateTime.Now;
                //    db.Entry(quest).State = System.Data.Entity.EntityState.Modified;
                //    db.SaveChanges();
                //    int questID = quest.Id;
                //    quest = db.Questions.Find(questID);
                //    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, quest, "Question"));
                //}
                //else
                //{
                quest = new Question();
                Token token = new Token();
                quest.PostQuestion = postQuestion.PostQuestion;
                quest.OwnerUserID = postQuestion.OwnerUserID;
                quest.HashTags = postQuestion.HashTags;
                quest.IsDeleted = false;
                quest.CreationDate = DateTime.Now;
                quest.TaggedUser = postQuestion.TaggedUser;
                db.Questions.Add(quest);
                db.SaveChanges();
                token = db.Tokens.Where(p => p.UserId == postQuestion.OwnerUserID).FirstOrDefault();
                token.BalanceToken = token.BalanceToken - 1;

                db.Entry(token).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                int questID = quest.Id;
                quest = db.Questions.Find(questID);
                return ObjToken;
                //return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, quest, "Question"));
                //}
            }
            catch (Exception ex)
            {
                return ObjToken;
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                //return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "Question"));
            }
        }
        #endregion

        [HttpPost]
        [Route("api/WebApi/GetAllNotificationByUser")]
        public List<UserNotifications> GetAllNotificationByUser(PagingModel Model)
        {
            List<UserNotifications> userNotifications2 = new List<UserNotifications>();
            try
            {


                int Total = Model.TotalRecords;
                int pageSize = 10; // set your page size, which is number of records per page
                int page = Model.PageNumber;
                int skip = pageSize * (page - 1);

                UserNotifications userNotifications = new UserNotifications();
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return userNotifications2;
                }

                var TotalRecordNotification = (from q1 in db.Questions
                                               join n1 in db.Notifications on q1.Id equals n1.questId
                                               where q1.OwnerUserID == Model.UserId
                                               //     select new UserNotifications { TotalRecordcount = db.Notifications.Count(y1 => y1.Id == n1.Id) }).ToList();
                                               select new UserNotifications { TotalRecordcount = n1.Id }).ToList().Count();

                var userNotifications1 = (from q in db.Questions
                                          join o in db.Opinions on q.Id equals o.QuestId
                                          join n in db.Notifications on o.Id equals n.CommentId
                                          join u in db.Users on o.CommentedUserId equals u.UserID
                                          where q.OwnerUserID == Model.UserId && q.IsDeleted == false
                                          select new UserNotifications
                                          {
                                              QuestionId = q.Id,
                                              Question = q.PostQuestion,
                                              HashTags = q.HashTags,
                                              OpinionId = o.Id,
                                              Opinion = o.Comment,
                                              Image = u.ImageURL,
                                              CommentedUserId = o.CommentedUserId,
                                              UserName = u.UserName,
                                              Like = ((n.Like ?? false) ? true : false),
                                              Dislike = ((n.Dislike ?? false) ? true : false),
                                              Comment = ((n.Comment ?? false) ? true : false),
                                              CreationDate = n.CreationDate,
                                              ModifiedDate = n.ModifiedDate,
                                              TotalRecordcount = TotalRecordNotification,
                                              NotificationId = n.Id,
                                          }).ToList().OrderByDescending(x => x.NotificationId).Skip(skip).Take(pageSize).ToList();

                foreach (var data in userNotifications1)
                {
                    data.Message = GenerateTags(data.Like, data.Dislike, data.Comment, data.UserName);
                    data.Tag = (data.Like == true) ? "Like" : (data.Dislike == true) ? "Dislike" : (data.Comment == true) ? "Comment" : "";
                }
                return userNotifications1.Where(p => p.Message != "").ToList();
                // return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, userNotifications1, "AllOpinion"));
            }
            catch (Exception ex)
            {
                return userNotifications2;
            }
        }


        public string GenerateTags(bool? like, bool? dislike, bool? comment, string UserName)
        {
            string Tag = "";
            if (like == true && dislike == false && comment == false)
            {
                Tag = UserName + " Has Liked your opinion.";
            }
            else if (dislike == true && like == false && comment == false)
            {
                Tag = UserName + " Has Disliked your opinion.";
            }
            else if (comment == true && like == false && dislike == false)
            {
                Tag = UserName + " Has given opinion on your Question.";
            }
            else if (like == true && dislike == false && comment == true)
            {
                Tag = UserName + " Has Liked and given opinion on your Question.";
            }
            else if (dislike == true && like == false && comment == true)
            {
                Tag = UserName + " Has Disliked and given opinion on your Question.";
            }

            return Tag;
        }



        #region "Get All Posts" 
        [HttpPost]
        [Route("api/WebApi/GetAllPostsWeb")]
        public List<PostQuestionDetailWebModel> GetAllPostsWeb(PagingModel model)
        {
            //    AllUserQuestions questionDetail = new AllUserQuestions();

            model.Search = model.Search ?? "";

            int Total = model.TotalRecords;
            int pageSize = 10; // set your page size, which is number of records per page
            int page = model.PageNumber;
            int skip = pageSize * (page - 1);

            //int canPage = skip < Total;


            List<PostQuestionDetailWebModel> questionDetail = new List<PostQuestionDetailWebModel>();
            try
            {
                db.Configuration.LazyLoadingEnabled = false;


                questionDetail = (from q in db.Questions
                                  join u in db.Users on q.OwnerUserID equals u.UserID
                                  where q.IsDeleted == false && q.PostQuestion.Contains(model.Search)
                                  select new PostQuestionDetailWebModel
                                  {
                                      Id = q.Id,
                                      Question = q.PostQuestion,
                                      OwnerUserID = q.OwnerUserID,
                                      OwnerUserName = u.UserName,
                                      Name = u.FirstName + " " + u.LastName,
                                      UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                      HashTags = q.HashTags,
                                      CreationDate = q.CreationDate,
                                      YesCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == true).Count(),
                                      NoCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == false).Count(),
                                      TotalLikes = db.Notifications.Where(o => o.questId == q.Id && o.Like == true).Count(),
                                      TotalDisLikes = db.Notifications.Where(o => o.questId == q.Id && o.Dislike == true).Count(),
                                      TotalRecordcount = db.Questions.Count(x => x.IsDeleted == false && x.PostQuestion.Contains(model.Search))

                                  }).OrderByDescending(p => p.Id).Skip(skip).Take(pageSize).ToList();




                foreach (var data in questionDetail)
                {
                    var opinionList = db.Opinions.Where(p => p.QuestId == data.Id).ToList();
                    if (opinionList.Count > 0)
                    {

                        int? maxYesLike = opinionList.Where(p => p.IsAgree == true).Max(i => i.Likes);
                        int? maxNoLike = opinionList.Where(p => p.IsAgree == false).Max(i => i.Likes);
                        //int? maxDislike = opinionList.Max(i => i.Dislikes);
                        if (maxYesLike != null && maxYesLike > 0)
                        {
                            data.MostYesLiked = (from e in db.Opinions
                                                 join t in db.Users on e.CommentedUserId equals t.UserID
                                                 join n in db.Notifications on e.QuestId equals n.questId
                                                 where e.IsAgree == true && e.QuestId == data.Id && n.Like == true
                                                 select new Comments
                                                 {
                                                     Id = e.Id,
                                                     Comment = e.Comment,
                                                     CommentedUserId = t.UserID,
                                                     Name = t.FirstName + " " + t.LastName,
                                                     UserImage = string.IsNullOrEmpty(t.ImageURL) ? "" : t.ImageURL,
                                                     LikesCount = db.Notifications.Where(p => p.CommentId == e.Id && p.Like == true).Count(),
                                                     DislikesCount = db.Notifications.Where(p => p.CommentId == e.Id && p.Dislike == true).Count(),
                                                     CommentedUserName = t.UserName,
                                                     CreationDate = e.CreationDate
                                                 }).OrderByDescending(s => s.LikesCount).ThenByDescending(s => s.CreationDate).First();

                        }
                        if (maxNoLike != null && maxNoLike > 0)
                        {
                            data.MostNoLiked = (from e in db.Opinions
                                                join t in db.Users on e.CommentedUserId equals t.UserID
                                                join n in db.Notifications on e.QuestId equals n.questId
                                                where e.IsAgree == false && e.QuestId == data.Id && n.Like == true
                                                select new Comments
                                                {
                                                    Id = e.Id,
                                                    Comment = e.Comment,
                                                    CommentedUserId = t.UserID,
                                                    Name = t.FirstName + " " + t.LastName,
                                                    UserImage = string.IsNullOrEmpty(t.ImageURL) ? "" : t.ImageURL,
                                                    LikesCount = db.Notifications.Where(p => p.CommentId == e.Id && p.Like == true).Count(),
                                                    DislikesCount = db.Notifications.Where(p => p.CommentId == e.Id && p.Dislike == true).Count(),
                                                    CommentedUserName = t.UserName,
                                                    CreationDate = e.CreationDate
                                                }).OrderByDescending(s => s.LikesCount).ThenByDescending(s => s.CreationDate).First();
                        }
                    }
                }
                return questionDetail;
                //return Request.CreateResponse(JsonResponse.GetResponse(ResponseCode.Success, questionDetail, "AllUserQuestions"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return questionDetail;
            }
        }
        #endregion


        #region "Get All Opinion by question Id" 
        [HttpGet]
        [Route("api/WebApi/GetAllOpinionWeb")]
        public BookMarkQuestion GetAllOpinionWeb(string questId, int UserId)
        {
            BookMarkQuestion questionDetail = new BookMarkQuestion();
            try
            {
                using (oposeeDbEntities db = new oposeeDbEntities())
                {
                    if (!ModelState.IsValid)
                    {
                        return questionDetail;
                    }

                    int id = Convert.ToInt32(questId);
                    //int userId = Convert.ToInt32(userid);
                    questionDetail.PostQuestionDetail = (from q in db.Questions
                                                         join u in db.Users on q.OwnerUserID equals u.UserID
                                                         where q.Id == id && q.IsDeleted == false
                                                         select new BookMarkQuestionDetail
                                                         {
                                                             Id = q.Id,
                                                             Question = q.PostQuestion,
                                                             OwnerUserID = q.OwnerUserID,
                                                             OwnerUserName = u.UserName,
                                                             UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                                             HashTags = q.HashTags,
                                                             YesCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == true).Count(),
                                                             NoCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == false).Count(),
                                                             CreationDate = q.CreationDate,
                                                             IsBookmark = db.BookMarks.Where(b => b.UserId == UserId && b.QuestionId == id).Select(b => b.IsBookmark.HasValue ? b.IsBookmark.Value : false).FirstOrDefault(),
                                                         }).FirstOrDefault();
                    
                    questionDetail.Comments = this.SortedComments(id, UserId);

                    return questionDetail;
                    // return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, questionDetail, "AllOpinion"));
                }
            }
            catch (Exception ex)
            {
                return questionDetail;
                //  OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                //  return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "AllOpinion"));
            }
        }

        private List<Comments> SortedComments(int id, int UserId)
        {
            try
            {
                var cList = (from e in db.Opinions
                             join t in db.Users on e.CommentedUserId equals t.UserID
                             where e.QuestId == id
                             select new Comments
                             {
                                 Id = e.Id,
                                 Comment = e.Comment,
                                 CommentedUserId = t.UserID,
                                 Name = t.FirstName + " " + t.LastName,
                                 UserImage = string.IsNullOrEmpty(t.ImageURL) ? "" : t.ImageURL,
                                 LikesCount = db.Notifications.Where(p => p.CommentId == e.Id && p.Like == true).Count(),
                                 DislikesCount = db.Notifications.Where(p => p.CommentId == e.Id && p.Dislike == true).Count(),
                                 Likes = db.Notifications.Where(p => p.CommentedUserId == UserId && p.CommentId == e.Id).Select(b => b.Like.HasValue ? b.Like.Value : false).FirstOrDefault(),
                                 DisLikes = db.Notifications.Where(p => p.CommentedUserId == UserId && p.CommentId == e.Id).Select(b => b.Dislike.HasValue ? b.Dislike.Value : false).FirstOrDefault(),
                                 CommentedUserName = t.UserName,
                                 IsAgree = e.IsAgree,
                                 CreationDate = e.CreationDate
                             }).ToList();

                var YesComments = cList.Where(x => x.IsAgree == true).OrderByDescending(x => (x.LikesCount - x.DislikesCount)).ToList();
                var NoComments = cList.Where(x => x.IsAgree == false).OrderByDescending(x => (x.LikesCount - x.DislikesCount)).ToList();

                List<Comments> _commentList = new List<Comments>();
                for (var i = 1; i < cList.Count + 1; i++)
                {
                    Comments comment = null;

                    if (i % 2 == 0) //even=no
                    {
                        if (NoComments.Count > 0)
                        {
                            comment = NoComments[0];
                            NoComments.Remove(comment);
                        }
                        else if (YesComments.Count > 0)
                        {
                            comment = YesComments[0];
                            YesComments.Remove(comment);
                        }
                    }
                    else
                    {
                        if (YesComments.Count > 0)
                        {
                            comment = YesComments[0];
                            YesComments.Remove(comment);
                        }
                        else if (NoComments.Count > 0)
                        {
                            comment = NoComments[0];
                            NoComments.Remove(comment);
                        }
                    }

                    if (comment != null)
                        _commentList.Add(comment);
                }

                return _commentList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("api/WebApi/BookMarkQuestionWeb")]
        public HttpResponseMessage BookMarkQuestionWeb(QuestionBookmarkWebModel questionBookmark)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                BookMark quest = null;

                quest = db.BookMarks.Where(p => p.QuestionId == questionBookmark.QuestionId).FirstOrDefault();
                if (quest != null)
                {
                    quest.IsBookmark = questionBookmark.IsBookmark;
                    quest.UserId = questionBookmark.UserId;
                    quest.QuestionId = questionBookmark.QuestionId;
                    quest.ModifiedDate = DateTime.Now;
                    db.Entry(quest).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    int questID = quest.Id;
                    quest = db.BookMarks.Find(questID);
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, quest, "UserData"));
                }
                else
                {
                    quest = new BookMark();

                    quest.IsBookmark = questionBookmark.IsBookmark;
                    quest.UserId = questionBookmark.UserId;
                    quest.QuestionId = questionBookmark.QuestionId;
                    quest.CreationDate = DateTime.Now;
                    db.BookMarks.Add(quest);
                    db.SaveChanges();
                    int questID = quest.Id;
                    quest = db.BookMarks.Find(questID);
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, quest, "Question"));
                }
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "Question"));
            }
        }

        [HttpGet]
        [Route("api/WebApi/GetAllBookMarkWebById")]
        public List<PostQuestionDetailWebModel> GetAllBookMarkWebById(int userId)
        {
            List<PostQuestionDetailWebModel> questionDetail = new List<PostQuestionDetailWebModel>();
            try
            {
                db.Configuration.LazyLoadingEnabled = false;


                //int id = Convert.ToInt32(userId);

                questionDetail = (from q in db.Questions
                                  join b in db.BookMarks on q.Id equals b.QuestionId
                                  join u in db.Users on b.UserId equals u.UserID
                                  where q.IsDeleted == false && u.UserID == userId && b.IsBookmark == true
                                  select new PostQuestionDetailWebModel
                                  {
                                      Id = q.Id,
                                      Question = q.PostQuestion,
                                      OwnerUserID = q.OwnerUserID,
                                      OwnerUserName = u.UserName,
                                      UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                      HashTags = q.HashTags,
                                      Name = u.FirstName + " " + u.LastName,
                                      TotalLikes = db.Notifications.Where(o => o.questId == q.Id && o.Like == true).Count(),
                                      TotalDisLikes = db.Notifications.Where(o => o.questId == q.Id && o.Dislike == true).Count(),
                                      CreationDate = q.CreationDate,
                                      YesCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == true).Count(),
                                      NoCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == false).Count()
                                  }).OrderByDescending(p => p.CreationDate).ToList();

                return questionDetail;

                // return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, questionDetail, "GetBookmarkQuestion"));
            }
            catch (Exception ex)
            {
                return questionDetail;
                //return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "GetBookmarkQuestion"));
            }
        }







        #endregion
        #region "Get User Profile" 
        [HttpGet]
        [Route("api/WebApi/GetUserProfileWeb")]
        public UserProfile GetUserProfileWeb(int userid)
        {
            UserProfile UserProfile = new UserProfile();
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    //   return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }

                //int id = Convert.ToInt32(UserID);

                UserProfile = (from t in db.Tokens
                               join u in db.Users on t.UserId equals u.UserID
                               where u.UserID == userid
                               select new UserProfile
                               {
                                   UserID = u.UserID,
                                   UserName = u.UserName,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   Email = u.Email,
                                   ImageURL = u.ImageURL,
                                   BalanceToken = t.BalanceToken,
                                   TotalPostedQuestion = db.Questions.Where(p => p.OwnerUserID == userid && p.IsDeleted == false).Count(),
                                   TotalLikes = (from q in db.Questions
                                                 join o in db.Opinions on q.Id equals o.QuestId
                                                 where q.OwnerUserID == userid && q.IsDeleted == false
                                                 select o.Likes).Sum(),
                                   TotalDislikes = (from q in db.Questions
                                                    join o in db.Opinions on q.Id equals o.QuestId
                                                    where q.OwnerUserID == userid && q.IsDeleted == false
                                                    select o.Dislikes).Sum(),
                               }).FirstOrDefault();

                return UserProfile;

                // return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, UserProfile, "UserProfile"));
            }
            catch (Exception ex)
            {
                return UserProfile;
                //  OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                //  return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "UserProfile"));
            }
        }
        #endregion



        #region "Get User Profile" 
        [HttpGet]
        [Route("api/WebApi/GetEditUserProfileWeb")]
        public UserModelProfileEditWeb GetEditUserProfileWeb(int userid)
        {
            UserModelProfileEditWeb UserProfile = new UserModelProfileEditWeb();
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    //   return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }

                //int id = Convert.ToInt32(UserID);

                UserProfile = (from u in db.Users
                               where u.UserID == userid
                               select new UserModelProfileEditWeb
                               {
                                   UserId = u.UserID,
                                   UserName = u.UserName,
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   Email = u.Email,
                                   Password = u.Password,
                                   ImageURL = u.ImageURL
                               }).FirstOrDefault();

                UserProfile.Password = AesCryptography.Decrypt(UserProfile.Password);
                return UserProfile;

                //  UserProfile.Password

                // return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, UserProfile, "UserProfile"));
            }
            catch (Exception ex)
            {
                return UserProfile;
                //  OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                //  return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "UserProfile"));
            }
        }
        #endregion



        #region "Get User Profile" 
        [HttpPost]
        [Route("api/WebApi/EditUserProfileWeb")]
        public void EditUserProfileWeb(UserModelProfileEditWeb Model)
        {
            User UserProfile;
            try
            {
                UserProfile = db.Users.Where(p => p.UserID == Model.UserId).FirstOrDefault();
                UserProfile.FirstName = Model.FirstName;
                UserProfile.LastName = Model.LastName;
                UserProfile.Email = Model.Email;
                UserProfile.Password = AesCryptography.Encrypt(Model.Password);
                db.Entry(UserProfile).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception ex)
            {
            }
            #endregion
        }



        [HttpPost]
        [Route("api/WebApi/PostOpinionWeb")]
        public Token PostOpinionWeb(PostAnswerWeb Model)
        {
            Token ObjToken = null;
            Opinion ObjOpinion = new Opinion();
            Notification notification = null;
            try
            {


                ObjToken = db.Tokens.Where(x => x.UserId == Model.CommentedUserId).FirstOrDefault();
                if (ObjToken.BalanceToken <= 0)
                {
                    return ObjToken;
                }

                Token token = new Token();
                ObjOpinion.QuestId = Model.QuestId;
                ObjOpinion.Comment = Model.Comment;
                ObjOpinion.CommentedUserId = Model.CommentedUserId;
                ObjOpinion.CreationDate = DateTime.Now;
                ObjOpinion.Likes = Model.Likes;
                ObjOpinion.IsAgree = Model.OpinionAgreeStatus;
                ObjOpinion.Dislikes = Model.Dislikes;
                db.Opinions.Add(ObjOpinion);
                db.SaveChanges();
                int CommentId = ObjOpinion.Id;
                token = db.Tokens.Where(p => p.UserId == Model.CommentedUserId).FirstOrDefault();
                token.BalanceToken = token.BalanceToken - 1;

                db.Entry(token).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                notification = new Notification();
                notification.CommentedUserId = Model.CommentedUserId;
                notification.CommentId = CommentId;
                notification.questId = Model.QuestId;
                notification.Like = Convert.ToBoolean(Model.Likes);
                notification.Dislike = Convert.ToBoolean(Model.Dislikes);
                notification.Comment = true;
                notification.CreationDate = DateTime.Now;
                db.Notifications.Add(notification);
                db.SaveChanges();


                //  }

            }
            catch (Exception ex)
            {

            }
            return ObjToken;
        }


        [HttpPost]
        [Route("api/WebApi/PostLikeDislikeWeb")]
        public void PostLikeDislikeWeb(PostLikeDislikeModel Model)
        {

            Opinion ObjOpinion = new Opinion();
            Notification notification = null;
            string action = "";
            PushNotifications pNoty = new PushNotifications();
            try
            {
                notification = db.Notifications.Where(x => x.CommentedUserId == Model.CommentedUserId && x.questId == Model.QuestId && x.CommentId == Model.CommentId).FirstOrDefault();

                if (notification == null)
                {


                    //if (Model.CommentStatus == CommentStatus.DisLike)
                    //{
                    //    notification.Dislike = true;
                    //    notification.Like = false;
                    //    action = "dislike";
                    //}
                    //else if (Model.CommentStatus == CommentStatus.Like)
                    //{
                    //    notification.Like = true;
                    //    notification.Dislike = false;
                    //    action = "like";
                    //}
                    //if (Model.CommentStatus == CommentStatus.RemoveLike)
                    //{
                    //    notification.Like = false;
                    //    action = "remove like";
                    //}
                    //else if (Model.CommentStatus == CommentStatus.RemoveDisLike)
                    //{
                    //    notification.Dislike = false;
                    //    action = "remove dislike";
                    //}


                    notification = new Notification();
                    notification.CommentedUserId = Model.CommentedUserId;
                    notification.CommentId = Model.CommentId;
                    notification.questId = Model.QuestId;
                    notification.Like = Convert.ToBoolean(Model.Likes);
                    notification.Dislike = Convert.ToBoolean(Model.Dislikes);
                    notification.CreationDate = Model.CreationDate;
                    db.Notifications.Add(notification);
                    db.SaveChanges();


                    ///notification to mobile app
                    if (Model.Likes == 0)
                    {
                        notification.Dislike = true;
                        notification.Like = false;
                        action = "dislike";
                    }
                    else if (Model.Likes == 1)
                    {
                        notification.Like = true;
                        notification.Dislike = false;
                        action = "like";
                    }
                    List<Opinion> opinion = db.Opinions.Where(p => p.Id == Model.CommentId).ToList();
                    int questId = opinion[0].QuestId;
                    Question ques = db.Questions.Where(p => p.Id == questId).FirstOrDefault();
                    User questOwner = db.Users.Where(u => u.UserID == ques.OwnerUserID).FirstOrDefault();
                    User user = db.Users.Where(u => u.UserID == notification.CommentedUserId).FirstOrDefault();
                    int OpinionUserID = opinion[0].CommentedUserId;
                    User commentOwner = db.Users.Where(u => u.UserID == OpinionUserID).FirstOrDefault();
                    if (questOwner != null && (!action.Contains("remove")))
                    {
                        if (ques.OwnerUserID != notification.CommentedUserId)
                        {
                            //***** Notification to question owner
                            string finalMessage = GenerateTagsForQuestionWeb(notification.Like, notification.Dislike, false, user.FirstName + " " + user.LastName);

                            pNoty.SendNotification_Android(questOwner.DeviceToken, finalMessage, "QD", questId.ToString());

                            //***** Notification to Tagged Users
                            string taggedUser = ques.TaggedUser;

                            if (!string.IsNullOrEmpty(taggedUser))
                            {
                                var roleIds = taggedUser.Split(',').Select(s => int.Parse(s));
                                foreach (int items in roleIds)
                                {
                                    if (notification.CommentedUserId != items)
                                    {
                                        User data = db.Users.Find(items);
                                        if (data != null)
                                        {
                                            string finalMessage1 = user.FirstName + " " + user.LastName + " has " + action + " question in which you're tagged in.";

                                            pNoty.SendNotification_Android(data.DeviceToken, finalMessage1, "QD", questId.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        else if (ques.OwnerUserID == notification.CommentedUserId)
                        {
                            //in this block notification will send to tagged users
                            string taggedUser = ques.TaggedUser;

                            if (!string.IsNullOrEmpty(taggedUser))
                            {
                                var roleIds = taggedUser.Split(',').Select(s => int.Parse(s));
                                foreach (int items in roleIds)
                                {
                                    User data = db.Users.Find(items);
                                    if (data != null)
                                    {
                                        string finalMessage = user.FirstName + " " + user.LastName + " has " + action + " question in which you're tagged in.";

                                        pNoty.SendNotification_Android(data.DeviceToken, finalMessage, "QD", questId.ToString());
                                    }
                                }
                            }
                        }
                        if (commentOwner.UserID != notification.CommentedUserId)
                        {
                            //***** Notification to question owner
                            string finalMessage = GenerateTagsForOpinionWeb(notification.Like, notification.Dislike, false, user.FirstName + " " + user.LastName);

                            pNoty.SendNotification_Android(commentOwner.DeviceToken, finalMessage, "QD", questId.ToString());
                        }
                    }
                }
                else
                {


                    notification.CommentedUserId = Model.CommentedUserId;
                    notification.CommentId = Model.CommentId;
                    notification.questId = Model.QuestId;
                    //if (Model.LikeOrDislke){
                    //    notification.Like = Convert.ToBoolean(Model.Likes);
                    //}
                    //else{
                    //    notification.Dislike = Convert.ToBoolean(Model.Dislikes);
                    //}
                    notification.Like = Convert.ToBoolean(Model.Likes);
                    notification.Dislike = Convert.ToBoolean(Model.Dislikes);
                    notification.CreationDate = Model.CreationDate;
                    db.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    ///notification to mobile app
                    if (Model.Likes == 0)
                    {
                        notification.Dislike = true;
                        notification.Like = false;
                        action = "dislike";
                    }
                    else if (Model.Likes == 1)
                    {
                        notification.Like = true;
                        notification.Dislike = false;
                        action = "like";
                    }



                    List<Opinion> opinion = db.Opinions.Where(p => p.Id == Model.CommentId).ToList();
                    int questId = opinion[0].QuestId;
                    Question ques = db.Questions.Where(p => p.Id == questId).FirstOrDefault();
                    User questOwner = db.Users.Where(u => u.UserID == ques.OwnerUserID).FirstOrDefault();
                    User user = db.Users.Where(u => u.UserID == notification.CommentedUserId).FirstOrDefault();
                    int OpinionUserID = opinion[0].CommentedUserId;
                    User commentOwner = db.Users.Where(u => u.UserID == OpinionUserID).FirstOrDefault();
                    if (questOwner != null && (!action.Contains("remove")))
                    {
                        if (ques.OwnerUserID != notification.CommentedUserId)
                        {
                            //***** Notification to question owner
                            string finalMessage = GenerateTagsForQuestionWeb(notification.Like, notification.Dislike, false, user.FirstName + " " + user.LastName);

                            pNoty.SendNotification_Android(questOwner.DeviceToken, finalMessage, "QD", questId.ToString());

                            //***** Notification to Tagged Users
                            string taggedUser = ques.TaggedUser;

                            if (!string.IsNullOrEmpty(taggedUser))
                            {
                                var roleIds = taggedUser.Split(',').Select(s => int.Parse(s));
                                foreach (int items in roleIds)
                                {
                                    if (notification.CommentedUserId != items)
                                    {
                                        User data = db.Users.Find(items);
                                        if (data != null)
                                        {
                                            string finalMessage1 = user.FirstName + " " + user.LastName + " has " + action + " question in which you're tagged in.";

                                            pNoty.SendNotification_Android(data.DeviceToken, finalMessage1, "QD", questId.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        else if (ques.OwnerUserID == notification.CommentedUserId)
                        {
                            //in this block notification will send to tagged users
                            string taggedUser = ques.TaggedUser;

                            if (!string.IsNullOrEmpty(taggedUser))
                            {
                                var roleIds = taggedUser.Split(',').Select(s => int.Parse(s));
                                foreach (int items in roleIds)
                                {
                                    User data = db.Users.Find(items);
                                    if (data != null)
                                    {
                                        string finalMessage = user.FirstName + " " + user.LastName + " has " + action + " question in which you're tagged in.";

                                        pNoty.SendNotification_Android(data.DeviceToken, finalMessage, "QD", questId.ToString());
                                    }
                                }
                            }
                        }
                        if (commentOwner.UserID != notification.CommentedUserId)
                        {
                            //***** Notification to question owner
                            string finalMessage = GenerateTagsForOpinionWeb(notification.Like, notification.Dislike, false, user.FirstName + " " + user.LastName);

                            pNoty.SendNotification_Android(commentOwner.DeviceToken, finalMessage, "QD", questId.ToString());
                        }
                    }

                }



            }
            catch (Exception ex)
            {

            }
        }

        public string GenerateTagsForQuestionWeb(bool? like, bool? dislike, bool? comment, string UserName)
        {
            string Tag = "";
            if (like == true && dislike == false && comment == false)
            {
                Tag = UserName + " has liked your question's opinion.";
            }
            else if (dislike == true && like == false && comment == false)
            {
                Tag = UserName + " has disliked your question's opinion.";
            }
            else if (comment == true && like == false && dislike == false)
            {
                Tag = UserName + " has given opinion on your question.";
            }

            return Tag;
        }
        public string GenerateTagsForOpinionWeb(bool? like, bool? dislike, bool? comment, string UserName)
        {
            string Tag = "";
            if (like == true && dislike == false && comment == false)
            {
                Tag = UserName + " has liked your opinion.";
            }
            else if (dislike == true && like == false && comment == false)
            {
                Tag = UserName + " has disliked your opinion.";
            }
            else if (comment == true && like == false && dislike == false)
            {
                Tag = UserName + " has given opinion on your question.";
            }

            return Tag;
        }

        public string GenerateTagsForTaggedUsersWeb(bool? like, bool? dislike, bool? comment, string ActionUserName)
        {
            string Tag = "";
            if (like == true && dislike == false && comment == false)
            {
                Tag = ActionUserName + " has liked question's opinion in which you're tagged in";
            }
            else if (dislike == true && like == false && comment == false)
            {
                Tag = ActionUserName + " has disliked question's opinion in which you're tagged in";
            }
            else if (comment == true && like == false && dislike == false)
            {
                Tag = ActionUserName + " has given opinion on question in which you're tagged in";
            }
            //else if (like == true && dislike == false && comment == true)
            //{
            //    Tag = UserName + " Has Liked and given opinion on your Question.";
            //}
            //else if (dislike == true && like == false && comment == true)
            //{
            //    Tag = UserName + " Has Disliked and given opinion on your Question.";
            //}

            return Tag;
        }


        #region "Get All Search Users" 
        [HttpGet]
        [Route("api/WebApi/GetAllTaggedDropWeb")]
        public List<ViewModelUser> GetAllTaggedDropWeb()
        {
            List<ViewModelUser> user = new List<ViewModelUser>();
            try
            {

                user = (from u in db.Users
                        where u.IsAdmin == false
                        select new ViewModelUser
                        {
                            UserID = u.UserID,
                            UserName = u.UserName,
                            Name = u.FirstName + " " + u.LastName,
                            ImageURL = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                            CreatedDate = u.CreatedDate
                        }).OrderBy(p => p.UserID).ToList();


                return user;
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return user;
            }
        }
        #endregion



        [HttpPost]
        [Route("api/WebApi/UploadProfileWeb")]
        public HttpResponseMessage UploadProfileWeb()
        {
            string imageName = null;
            string _SiteRoot = WebConfigurationManager.AppSettings["SiteImgPath"];
            string _SiteURL = WebConfigurationManager.AppSettings["SiteImgURL"];

            var httpRequest = HttpContext.Current.Request;
            //Upload Image
            var postedFile = httpRequest.Files["Image"];
            int UserId = Convert.ToInt32(httpRequest["userId"]);
            //Create custom filename
            try
            {


                imageName = new String(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                string guid = Guid.NewGuid().ToString();
                imageName = imageName + guid + Path.GetExtension(postedFile.FileName);
                var filePath = HttpContext.Current.Server.MapPath("~/Content/upload/ProfileImage/" + imageName);
                postedFile.SaveAs(filePath);
                // ResizeImage.Resize_Image_Thumb(filePath, filePath, "_T_" + filePath, 400, 400);



                //System.Drawing.Image image = System.Drawing.Image.FromFile(filePath);
                //float aspectRatio = (float)image.Size.Width / (float)image.Size.Height;
                //int newHeight = 200;
                //int newWidth = Convert.ToInt32(aspectRatio * newHeight);
                //System.Drawing.Bitmap thumbBitmap = new System.Drawing.Bitmap(newWidth, newHeight);
                //System.Drawing.Graphics thumbGraph = System.Drawing.Graphics.FromImage(thumbBitmap);
                //thumbGraph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                //thumbGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //thumbGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //var imageRectangle = new Rectangle(400, 400, newWidth, newHeight);
                //thumbGraph.DrawImage(image, imageRectangle);
                //thumbBitmap.Save(filePath);
                //thumbGraph.Dispose();
                //thumbBitmap.Dispose();
                //image.Dispose();


                // Save to DB
                User Entry = null;
                using (oposeeDbEntities db = new oposeeDbEntities())
                {
                    Entry = db.Users.Where(x => x.UserID == UserId).FirstOrDefault();

                    Entry.ImageURL = _SiteURL + "/ProfileImage/" + imageName;
                    db.Entry(Entry).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //int userID = entity.UserID;
                    //entity = db.Users.Find(userID);


                }
            }
            catch (Exception exp)
            {

                throw;
            }
            return Request.CreateResponse(HttpStatusCode.Created);
        }
        public Image resizeImage(int newWidth, int newHeight, string stPhotoPath)
        {
            Image imgPhoto = Image.FromFile(stPhotoPath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            //Consider vertical pics
            if (sourceWidth < sourceHeight)
            {
                int buff = newWidth;
                newWidth = newHeight;
                newHeight = buff;
            }

            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = ((float)newWidth / (float)sourceWidth);
            nPercentH = ((float)newHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((newWidth -
                          (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((newHeight -
                          (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                          PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                         imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return bmPhoto;
        }



        #region "Socail Login" 
        [HttpPost]
        [Route("api/WebApi/SigninThirdPartyWeb")]
        public UserLoginWeb SigninThirdPartyWeb(InputSignInWithThirdPartyWebModel input)
        {
            UserLoginWeb ObjLogin = new UserLoginWeb();
            try
            {
                if (!ModelState.IsValid)
                {
                    //  return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }

                User entity = null;
                if (input.ThirdPartyType == ThirdPartyType.Facebook)
                {
                    entity = db.Users.Where(p => p.SocialID == input.ThirdPartyId
                                        && p.RecordStatus != RecordStatus.Deleted.ToString()).FirstOrDefault();
                }
                else if (input.ThirdPartyType == ThirdPartyType.Twitter)
                {
                    entity = db.Users.Where(p => p.SocialID == input.ThirdPartyId
                                        && p.RecordStatus != RecordStatus.Deleted.ToString()).FirstOrDefault();
                }
                else if (input.ThirdPartyType == ThirdPartyType.GooglePlus)
                {
                    entity = db.Users.Where(p => p.SocialID == input.ThirdPartyId
                                        && p.RecordStatus != RecordStatus.Deleted.ToString()).FirstOrDefault();
                }
                string strThumbnailURLfordb = null;
                string strIamgeURLfordb = null;
                string _SiteRoot = WebConfigurationManager.AppSettings["SiteImgPath"];
                string _SiteURL = WebConfigurationManager.AppSettings["SiteImgURL"];

                string strThumbnailImage = input.ImageURL;
                if (entity != null)
                {

                    if (entity.RecordStatus != RecordStatus.Active.ToString())
                    {
                        //return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Info, "User is not Active"));
                    }

                    entity.UserName = input.UserName != null && input.UserName != "" ? input.UserName : entity.UserName;
                    if (!string.IsNullOrEmpty(input.Password))
                    {
                        entity.Password = AesCryptography.Encrypt(input.Password);
                    }
                    entity.DeviceType = input.DeviceType != null && input.DeviceType != "" ? input.DeviceType : entity.DeviceType;
                    entity.DeviceToken = input.DeviceToken != null && input.DeviceToken != "" ? input.DeviceToken : entity.DeviceToken;
                    entity.ImageURL = entity.ImageURL;
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    int userID = entity.UserID;
                    entity = db.Users.Find(userID);

                    ObjLogin.Id = entity.UserID;
                    ObjLogin.Email = entity.Email;
                    ObjLogin.ImageURL = entity.ImageURL;
                    return ObjLogin;
                    //  return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, entity, "UserData"));
                }
                else
                {
                    entity = new User();
                    Token token = new Token();
                    entity.UserName = input.FirstName + input.LastName;
                    entity.FirstName = input.FirstName;
                    entity.LastName = input.LastName;
                    entity.Email = input.Email;

                    bool Email = false;
                    Email = OposeeLibrary.Utilities.Helper.IsValidEmail(input.Email);
                    if (!string.IsNullOrEmpty(input.Password))
                    {
                        entity.Password = AesCryptography.Encrypt(input.Password);
                    }

                    entity.DeviceType = input.DeviceType;
                    entity.DeviceToken = input.DeviceToken;
                    entity.CreatedDate = DateTime.Now;
                    entity.RecordStatus = RecordStatus.Active.ToString();
                    entity.SocialID = input.ThirdPartyId;
                    if (input.ThirdPartyType == ThirdPartyType.Facebook)
                    {
                        entity.SocialType = ThirdPartyType.Facebook.ToString();
                    }
                    else if (input.ThirdPartyType == ThirdPartyType.GooglePlus)
                    {
                        entity.SocialType = ThirdPartyType.GooglePlus.ToString();
                    }
                    else if (input.ThirdPartyType == ThirdPartyType.Twitter)
                    {
                        entity.SocialType = ThirdPartyType.Twitter.ToString();
                    }

                    if (input.ImageURL != null && input.ImageURL != "")
                    {
                        try
                        {
                            string strTempImageSave = OposeeLibrary.Utilities.ResizeImage.Download_Image(input.ImageURL);
                            string profileFilePath = _SiteURL + "/ProfileImage/" + strTempImageSave;
                            strIamgeURLfordb = profileFilePath;
                            entity.ImageURL = profileFilePath;
                        }
                        catch (Exception ex)
                        {
                            strThumbnailURLfordb = strThumbnailImage;
                            strIamgeURLfordb = strThumbnailImage;
                        }
                    }
                    else
                    {
                        entity.ImageURL = _SiteURL + "/ProfileImage/oposee-profile.png";
                    }
                    // entity.ImageURL = strIamgeURLfordb;
                    db.Users.Add(entity);
                    db.SaveChanges();

                    int userID = entity.UserID;
                    token.TotalToken = 100;
                    token.BalanceToken = 100;
                    token.UserId = userID;
                    db.Tokens.Add(token);
                    db.SaveChanges();
                    entity = db.Users.Find(userID);

                    ObjLogin.Id = entity.UserID;
                    ObjLogin.Email = entity.Email;
                    ObjLogin.ImageURL = entity.ImageURL;
                    return ObjLogin;

                    // return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, entity, "UserData"));
                }
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return ObjLogin;
                //return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "UserData"));
            }
        }
        #endregion


        #region "Get All Posts" 
        [HttpPost]
        [Route("api/WebApi/GetAllPostsQuestionEditWeb")]
        public List<PostQuestionDetailWebModel> GetAllPostsQuestionEditWeb(PagingModel model)
        {
            //    AllUserQuestions questionDetail = new AllUserQuestions();

            model.Search = model.Search ?? "";

            int Total = model.TotalRecords;
            int pageSize = 10; // set your page size, which is number of records per page
            int page = model.PageNumber;
            int skip = pageSize * (page - 1);

            //int canPage = skip < Total;


            List<PostQuestionDetailWebModel> questionDetail = new List<PostQuestionDetailWebModel>();
            try
            {
                db.Configuration.LazyLoadingEnabled = false;


                questionDetail = (from q in db.Questions
                                  join u in db.Users on q.OwnerUserID equals u.UserID
                                  where q.IsDeleted == false && q.OwnerUserID == model.UserId  // && q.PostQuestion.Contains(model.Search)
                                  select new PostQuestionDetailWebModel
                                  {
                                      Id = q.Id,
                                      Question = q.PostQuestion,
                                      OwnerUserID = q.OwnerUserID,
                                      OwnerUserName = u.UserName,
                                      Name = u.FirstName + " " + u.LastName,
                                      UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                      HashTags = q.HashTags,
                                      CreationDate = q.CreationDate,
                                      YesCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == true).Count(),
                                      NoCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == false).Count(),
                                      TotalLikes = db.Notifications.Where(o => o.questId == q.Id && o.Like == true).Count(),
                                      TotalDisLikes = db.Notifications.Where(o => o.questId == q.Id && o.Dislike == true).Count(),
                                      TotalRecordcount = db.Questions.Count(x => x.IsDeleted == false && x.OwnerUserID == model.UserId)

                                  }).OrderByDescending(p => p.Id).Skip(skip).Take(pageSize).ToList();



          
                return questionDetail;
                //return Request.CreateResponse(JsonResponse.GetResponse(ResponseCode.Success, questionDetail, "AllUserQuestions"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return questionDetail;
            }
        }
        #endregion



        #region "Get All Posts" 
        [HttpGet]
        [Route("api/WebApi/GetPostedQuestionEditWeb")]
        public PostQuestionModel GetPostedQuestionEditWeb(int QuestionId)
        {

           PostQuestionModel questionDetail = new PostQuestionModel();
            try
            {
                db.Configuration.LazyLoadingEnabled = false;


                questionDetail = (from q in db.Questions
                                  join u in db.Users on q.OwnerUserID equals u.UserID
                                  where q.Id == QuestionId
                                  select new PostQuestionModel
                                  {
                                      Id = q.Id,
                                      PostQuestion = q.PostQuestion,
                                      TaggedUser = q.TaggedUser,
                                      HashTags = q.HashTags,


                                  }).FirstOrDefault();

                return questionDetail;
                //return Request.CreateResponse(JsonResponse.GetResponse(ResponseCode.Success, questionDetail, "AllUserQuestions"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return questionDetail;
            }
        }
        #endregion

        #region "Post Question" 
        [HttpPost]
        [Route("api/WebApi/EditPostQuestionWeb")]
        public Question EditPostQuestionWeb([FromBody] PostQuestionModel postQuestion)
        {
       
            Question quest = null;
            try
            {

                if (!ModelState.IsValid)
                {
                    return quest; ;
                }
          
                quest = db.Questions.Where(p => p.Id == postQuestion.Id && p.OwnerUserID == postQuestion.OwnerUserID).FirstOrDefault();
                if(quest == null)
                {
                    return quest;
                }
                //quest = new Question();
                quest.PostQuestion = postQuestion.PostQuestion;
                quest.HashTags = postQuestion.HashTags;
                quest.ModifiedDate= DateTime.Now;
                db.Entry(quest).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
 
               return quest;
                //return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, quest, "Question"));
                //}
            }
            catch (Exception ex)
            {
                return quest;
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                //return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "Question"));
            }
        }
        #endregion


        #region "Post Question" 
        [HttpPost]
        [Route("api/WebApi/DeletePostQuestionWeb")]
        public Question DeletePostQuestionWeb( PostQuestionModel postQuestion)
        {

            Question quest = null;
            try
            {
                quest = db.Questions.Where(p => p.Id == postQuestion.Id && p.OwnerUserID == postQuestion.OwnerUserID).FirstOrDefault();
                if (quest == null)
                {
                    return quest;
                }
                //quest = new Question();
                quest.IsDeleted = true;
                quest.ModifiedDate = DateTime.Now;
                db.Entry(quest).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return quest;
              
            }
            catch (Exception ex)
            {
                return quest;
                
            }
        }
        #endregion


    }
}