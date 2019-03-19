using oposee.Enums;
using oposee.Models;
using oposee.Models.API;
using oposee.Models.Models;
using OposeeLibrary.API;
using OposeeLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                    token.TotalToken = 50;
                    token.BalanceToken = 50;
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
                var v = db.Users.Where(a => a.Email == login.Email && (a.IsAdmin ?? false) == false ).FirstOrDefault();
                if (v != null)
                {
                    ObjLogin.Token =  AesCryptography.Encrypt(login.Password);
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
        public HttpResponseMessage PostQuestionWeb([FromBody] Question postQuestion)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
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
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, quest, "Question"));
                //}
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "Question"));
            }
        }
        #endregion

        [HttpGet]
        [Route("api/WebApi/GetAllNotificationByUser/{userId}")]
        public List<UserNotifications> GetAllNotificationByUser(string userId)
        {
            List<UserNotifications> userNotifications2 = new List<UserNotifications>();
            try
            {

                UserNotifications userNotifications = new UserNotifications();
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return userNotifications2;
                }

                int id = Convert.ToInt32(userId);

                var userNotifications1 = (from q in db.Questions
                                          join o in db.Opinions on q.Id equals o.QuestId
                                          join n in db.Notifications on o.Id equals n.CommentId
                                          join u in db.Users on o.CommentedUserId equals u.UserID
                                          where q.OwnerUserID == id && q.IsDeleted == false
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
                                              ModifiedDate = n.ModifiedDate
                                          }).ToList();

                foreach (var data in userNotifications1)
                {
                    data.Message = GenerateTags(data.Like, data.Dislike, data.Comment, data.UserName);
                    data.Tag = (data.Like == true) ? "Like" : (data.Dislike == true) ? "Dislike" : (data.Comment == true) ? "Comment" : "";
                }
                return userNotifications1;
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
        public List<PostQuestionDetailWebModel> GetAllPostsWeb(QuestionGetModel model)
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
                                      TotalRecordcount = db.Questions.Count()

                                       }).OrderByDescending(p => p.CreationDate).Skip(skip).Take(pageSize).ToList();


        

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
        public BookMarkQuestion GetAllOpinionWeb(string questId,int UserId)
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


                    questionDetail.Comments = (from e in db.Opinions
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
                                                   //Likes = db.Notifications.Where(p => p.CommentedUserId == userId && p.CommentId == e.Id).Select(b => b.Like.HasValue ? b.Like.Value : false).FirstOrDefault(),
                                                   // DisLikes = db.Notifications.Where(p => p.CommentedUserId == userId && p.CommentId == e.Id).Select(b => b.Dislike.HasValue ? b.Dislike.Value : false).FirstOrDefault(),
                                                   CommentedUserName = t.UserName,
                                                   IsAgree = e.IsAgree,
                                                   CreationDate = e.CreationDate
                                               }).ToList();
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
                                                     where q.IsDeleted == false && u.UserID == userId
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
        public void PostOpinionWeb(PostAnswerWeb Model)
        {

            Opinion ObjOpinion = new Opinion();
            Notification notification = null;
            try
            {

                //ObjOpinion = db.Opinions.Where(p => p.QuestId == Model.QuestId).FirstOrDefault();
                //if (ObjOpinion != null)
                //{

                //    ObjOpinion.QuestId = Model.QuestId;
                //    ObjOpinion.Comment = Model.Comment;
                //    ObjOpinion.CommentedUserId = Model.CommentedUserId;
                //    ObjOpinion.ModifiedDate = DateTime.Now;
                //    db.Entry(ObjOpinion).State = System.Data.Entity.EntityState.Modified;
                //    db.SaveChanges();
                //    int questID = ObjOpinion.Id;
                //    ObjOpinion = db.Opinions.Find(questID);
                //    notification = db.Notifications.Where(p => p.CommentedUserId == Model.CommentedUserId && p.CommentId == questID).FirstOrDefault();
                //    if (notification != null)
                //    {
                //        notification.CommentedUserId = Model.CommentedUserId;
                //        notification.CommentId = questID;

                //        notification.ModifiedDate = DateTime.Now;
                //        db.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                //        db.SaveChanges();
                //    }
                //    else
                //    {
                //        notification = new Notification();
                //        notification.CommentedUserId = Model.CommentedUserId;
                //        notification.CommentId = questID;

                //        notification.CreationDate = DateTime.Now;
                //        db.Notifications.Add(notification);
                //        db.SaveChanges();
                //    }
                //}
                //else
                //{
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

                notification.CreationDate = DateTime.Now;
                db.Notifications.Add(notification);
                db.SaveChanges();


                //  }

            }
            catch (Exception ex)
            {

            }
        }


        [HttpPost]
        [Route("api/WebApi/PostLikeDislikeWeb")]
        public void PostLikeDislikeWeb(PostLikeDislikeModel Model)
        {

            Opinion ObjOpinion = new Opinion();
            Notification notification = null;
            try
            {
                 notification = db.Notifications.Where(x => x.CommentedUserId == Model.CommentedUserId && x.questId == Model.QuestId && x.CommentId == Model.CommentId).FirstOrDefault();

                if (notification == null)
                {
                    notification = new Notification();
                    notification.CommentedUserId = Model.CommentedUserId;
                    notification.CommentId = Model.CommentId;
                    notification.questId = Model.QuestId;
                    notification.Like = Convert.ToBoolean(Model.Likes);
                    notification.Dislike = Convert.ToBoolean(Model.Dislikes);
                    notification.CreationDate = Model.CreationDate;
                    db.Notifications.Add(notification);
                    db.SaveChanges();
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


                }

            

            }
            catch (Exception ex)
            {

            }
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

    }
}