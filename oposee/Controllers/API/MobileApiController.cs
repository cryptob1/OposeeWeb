using oposee.Enums;
using oposee.Models.API;
using oposee.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using OposeeLibrary.API;
using static OposeeLibrary.Utilities.ResizeImage;
using OposeeLibrary.Utilities;
using System.Data.Entity;
using System.Data.SqlClient;

namespace oposee.Controllers.API
{
    [RoutePrefix("oposee")]
    public class MobileApiController : ApiController
    {
        oposeeDbEntities db = new oposeeDbEntities();

        int _Imagethumbsize = 0;
        int _imageSize = 0;
        // GET: api/MobileApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MobileApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/MobileApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/MobileApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MobileApi/5
        public void Delete(int id)
        {
        }

        #region "Socail Login" 
        [HttpPost]
        [Route("api/MobileApi/signinthirdparty")]
        public HttpResponseMessage SigninThirdParty(InputSignInWithThirdParty input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
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
                    entity.ImageURL = "";
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

                        }
                    }
                    else
                    {
                        entity.ImageURL = _SiteURL + "/ProfileImage/oposee-profile.png";
                    }
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    int userID = entity.UserID;
                    entity = db.Users.Find(userID);
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, entity, "UserData"));
                }
                else
                {
                    entity = new User();
                    Token token = new Token();
                    entity.UserName = input.UserName;
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
                    else { entity.ImageURL = _SiteURL + "/ProfileImage/oposee-profile.png"; }
                    entity.ImageURL = strIamgeURLfordb;
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
        #endregion

        #region "Post Question" 
        [HttpPost]
        [Route("api/MobileApi/PostQuestion")]
        public HttpResponseMessage PostQuestion([FromBody] PostQuestion postQuestion)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                Question quest = null;
                quest = db.Questions.Where(p => p.Id == postQuestion.Id
                                       ).FirstOrDefault();
                if (quest != null)
                {
                    quest.PostQuestion = postQuestion.Question;
                    quest.OwnerUserID = postQuestion.OwnerUserID;
                    quest.HashTags = postQuestion.HashTags;
                    quest.ModifiedDate = DateTime.Now;
                    db.Entry(quest).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    int questID = quest.Id;
                    quest = db.Questions.Find(questID);
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, quest, "UserData"));
                }
                else
                {
                    quest = new Question();
                    Token token = new Token();
                    quest.PostQuestion = postQuestion.Question;
                    quest.OwnerUserID = postQuestion.OwnerUserID;
                    quest.HashTags = postQuestion.HashTags;
                    quest.CreationDate = DateTime.Now;
                    db.Questions.Add(quest);
                    db.SaveChanges();
                    token = db.Tokens.Where(p => p.UserId == postQuestion.OwnerUserID).FirstOrDefault();
                    token.BalanceToken = token.BalanceToken - 1;

                    db.Entry(token).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    int questID = quest.Id;
                    quest = db.Questions.Find(questID);
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, quest, "Question"));
                }
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "Question"));
            }
        }
        #endregion

        #region "Post Answer" 
        [HttpPost]
        [Route("api/MobileApi/PostAnswer")]
        public HttpResponseMessage PostAnswer([FromBody] PostAnswer postAnswer)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                Opinion opinion = null;
                Notification notification = null;
                opinion = db.Opinions.Where(p => p.Id == postAnswer.Id).FirstOrDefault();
                if (opinion != null)
                {
                    opinion.Comment = postAnswer.Comment;
                    opinion.QuestId = postAnswer.QuestId;
                    opinion.CommentedUserId = postAnswer.CommentedUserId;
                    opinion.ModifiedDate = DateTime.Now;
                    db.Entry(opinion).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    int questID = opinion.Id;
                    opinion = db.Opinions.Find(questID);
                    notification = db.Notifications.Where(p => p.CommentedUserId == postAnswer.CommentedUserId && p.CommentId == questID).FirstOrDefault();
                    if (notification != null)
                    {
                        notification.CommentedUserId = postAnswer.CommentedUserId;
                        notification.CommentId = questID;

                        notification.ModifiedDate = DateTime.Now;
                        db.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        notification = new Notification();
                        notification.CommentedUserId = postAnswer.CommentedUserId;
                        notification.CommentId = questID;

                        notification.CreationDate = DateTime.Now;
                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, opinion, "Opinion"));
                }
                else
                {
                    opinion = new Opinion();
                    opinion.Comment = postAnswer.Comment;
                    opinion.QuestId = postAnswer.QuestId;
                    opinion.CommentedUserId = postAnswer.CommentedUserId;
                    opinion.CreationDate = DateTime.Now;
                    db.Opinions.Add(opinion);
                    db.SaveChanges();
                    int questID = opinion.Id;
                    opinion = db.Opinions.Find(questID);
                    notification = db.Notifications.Where(p => p.CommentedUserId == postAnswer.CommentedUserId && p.CommentId == questID).FirstOrDefault();
                    if (notification != null)
                    {
                        notification.CommentedUserId = postAnswer.CommentedUserId;
                        notification.CommentId = questID;

                        notification.ModifiedDate = DateTime.Now;
                        db.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        notification = new Notification();
                        notification.CommentedUserId = postAnswer.CommentedUserId;
                        notification.CommentId = questID;
                        notification.Comment = true;
                        notification.CreationDate = DateTime.Now;
                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, opinion, "Opinion"));
                }
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "Opinion"));
            }
        }
        #endregion

        #region "Get All Opinion by question Id" 
        [HttpGet]
        [Route("api/MobileApi/GetAllOpinion")]
        public HttpResponseMessage GetAllOpinion(string questId, string userid)
        {
            try
            {
                using (oposeeDbEntities db = new oposeeDbEntities())
                {
                    if (!ModelState.IsValid)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                    }
                    BookMarkQuestion questionDetail = new BookMarkQuestion();
                    int id = Convert.ToInt32(questId);
                    int userId = Convert.ToInt32(userid);
                    questionDetail.PostQuestionDetail = (from q in db.Questions
                                                         join u in db.Users on q.OwnerUserID equals u.UserID
                                                         where q.Id == id
                                                         select new BookMarkQuestionDetail
                                                         {
                                                             Id = q.Id,
                                                             Question = q.PostQuestion,
                                                             OwnerUserID = q.OwnerUserID,
                                                             OwnerUserName = u.UserName,
                                                             UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                                             HashTags = q.HashTags,

                                                             //IsBookmark = db.BookMarks.Where(b => b.UserId == userId && b.QuestionId == id).Select(b => b.IsBookmark.HasValue ? b.IsBookmark.   Value:false ).FirstOrDefault(),
                                                             CreationDate = q.CreationDate
                                                         }).FirstOrDefault();


                    questionDetail.Comments = (from e in db.Opinions
                                               join t in db.Users on e.CommentedUserId equals t.UserID
                                               where e.QuestId == id
                                               select new Comments
                                               {
                                                   Id = e.Id,
                                                   Comment = e.Comment,
                                                   CommentedUserId = t.UserID,
                                                   UserImage = string.IsNullOrEmpty(t.ImageURL) ? "" : t.ImageURL,
                                                   LikesCount = db.Notifications.Where(p => p.CommentId == e.Id && p.Like == true).Count(),
                                                   DislikesCount = db.Notifications.Where(p => p.CommentId == e.Id && p.Dislike == true).Count(),
                                                   Likes = db.Notifications.Where(p => p.CommentedUserId == userId && p.CommentId == e.Id).Select(b => b.Like.HasValue ? b.Like.Value : false).FirstOrDefault(),
                                                   DisLikes = db.Notifications.Where(p => p.CommentedUserId == userId && p.CommentId == e.Id).Select(b => b.Dislike.HasValue ? b.Dislike.Value : false).FirstOrDefault(),
                                                   CommentedUserName = t.UserName,
                                                   CreationDate = e.CreationDate
                                               }).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, questionDetail, "AllOpinion"));
                }
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "AllOpinion"));
            }
        }
        #endregion

        #region "Get All Opinion by User Id" 
        [HttpGet]
        [Route("api/MobileApi/GetAllPostsByUserId")]
        public HttpResponseMessage GetAllPostsByUserId(string UserID)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                AllUserQuestions questionDetail = new AllUserQuestions();
                int id = Convert.ToInt32(UserID);

                questionDetail.PostQuestionDetail = (from q in db.Questions
                                                     join u in db.Users on q.OwnerUserID equals u.UserID
                                                     where u.UserID == id
                                                     select new PostQuestionDetail
                                                     {
                                                         Id = q.Id,
                                                         Question = q.PostQuestion,
                                                         OwnerUserID = q.OwnerUserID,
                                                         OwnerUserName = u.UserName,
                                                         UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                                         HashTags = q.HashTags,
                                                         CreationDate = q.CreationDate
                                                     }).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, questionDetail, "AllUserQuestions"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "AllUserQuestions"));
            }
        }
        #endregion

        #region "Get All Posts" 
        [HttpGet]
        [Route("api/MobileApi/GetAllPosts")]
        public HttpResponseMessage GetAllPosts()
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                AllUserQuestions questionDetail = new AllUserQuestions();
                //int id = Convert.ToInt32(UserID);

                questionDetail.PostQuestionDetail = (from q in db.Questions
                                                     join u in db.Users on q.OwnerUserID equals u.UserID
                                                     select new PostQuestionDetail
                                                     {
                                                         Id = q.Id,
                                                         Question = q.PostQuestion,
                                                         OwnerUserID = q.OwnerUserID,
                                                         OwnerUserName = u.UserName,
                                                         UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                                         HashTags = q.HashTags,
                                                         CreationDate = q.CreationDate
                                                     }).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, questionDetail, "AllUserQuestions"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "AllUserQuestions"));
            }
        }
        #endregion

        #region "Like Dislike Opinion" 
        [HttpPost]
        [Route("api/MobileApi/LikeDislikeOpinion")]
        public HttpResponseMessage LikeDislikeOpinion([FromBody] LikeDislike likeDislike)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                Notification notification = null;
                notification = db.Notifications.Where(p => p.CommentedUserId == likeDislike.CommentedUserId && p.CommentId == likeDislike.CommentId).FirstOrDefault();
                if (notification != null)
                {
                    if (likeDislike.CommentStatus == CommentStatus.DisLike)
                    {
                        notification.Dislike = true;
                        notification.Like = false;
                    }
                    else if (likeDislike.CommentStatus == CommentStatus.Like)
                    {
                        notification.Like = true;
                        notification.Dislike = false;
                    }
                    if (likeDislike.CommentStatus == CommentStatus.RemoveLike)
                    {
                        notification.Like = false;
                    }
                    else if (likeDislike.CommentStatus == CommentStatus.RemoveDisLike)
                    {
                        notification.Dislike = false;
                    }
                    notification.CommentedUserId = likeDislike.CommentedUserId;
                    notification.CommentId = likeDislike.CommentId;

                    notification.ModifiedDate = DateTime.Now;
                    db.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    List<Opinion> opinion = db.Opinions.Where(p => p.Id == likeDislike.CommentId).ToList();

                    foreach (Opinion orderID in opinion)
                    {
                        orderID.Likes = db.Notifications.Where(p => p.CommentId == orderID.Id && p.Like == true).Count();
                        orderID.Dislikes = db.Notifications.Where(p => p.CommentId == orderID.Id && p.Dislike == true).Count();
                        db.Entry(orderID).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }


                    int questID = notification.Id;
                    notification = db.Notifications.Find(questID);
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, notification, "LikeDislikeOpinion"));
                }
                else
                {
                    notification = new Notification();
                    if (likeDislike.CommentStatus == CommentStatus.DisLike)
                    {
                        notification.Dislike = true;
                        notification.Like = false;
                    }
                    else if (likeDislike.CommentStatus == CommentStatus.Like)
                    {
                        notification.Like = true;
                        notification.Dislike = false;
                    }
                    if (likeDislike.CommentStatus == CommentStatus.RemoveLike)
                    {
                        notification.Like = false;
                    }
                    else if (likeDislike.CommentStatus == CommentStatus.RemoveDisLike)
                    {
                        notification.Dislike = false;
                    }
                    notification.CommentedUserId = likeDislike.CommentedUserId;
                    notification.CommentId = likeDislike.CommentId;

                    notification.CreationDate = DateTime.Now;

                    db.Notifications.Add(notification);
                    db.SaveChanges();
                    int questID = notification.Id;
                    notification = db.Notifications.Find(questID);
                    List<Opinion> op = db.Opinions.Where(p => p.Id == likeDislike.CommentId).ToList();

                    foreach (Opinion orderID in op)
                    {
                        orderID.Likes = db.Notifications.Where(p => p.CommentId == orderID.Id && p.Like == true).Count();
                        orderID.Dislikes = db.Notifications.Where(p => p.CommentId == orderID.Id && p.Dislike == true).Count();
                        db.Entry(orderID).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }


                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, notification, "LikeDislikeOpinion"));
                }
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "LikeDislikeOpinion"));
            }
        }

        #endregion

        #region "Search by hashtag" 
        [HttpGet]
        [Route("api/MobileApi/SearchByHashTagOrString")]
        public HttpResponseMessage SearchByHashTagOrString(string search)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                //if (!ModelState.IsValid)
                //{
                //    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                //}

                if (search.Contains("~"))
                {
                    search = search.Replace("~", "#").ToString();
                }
                var result = (from q in db.Questions
                              join u in db.Users on q.OwnerUserID equals u.UserID
                              select new PostQuestionDetail
                              {
                                  Id = q.Id,
                                  Question = q.PostQuestion,
                                  OwnerUserID = q.OwnerUserID,
                                  OwnerUserName = u.UserName,
                                  UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                  HashTags = q.HashTags,
                                  CreationDate = q.CreationDate
                              }).Where(s => s.HashTags.Contains(search) || s.Question.Contains(search)).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, result, "SearchQuestion"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);

                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "SearchQuestion"));
            }
        }
        #endregion

        #region "BookMark Question" 
        [HttpPost]
        [Route("api/MobileApi/BookMarkQuestion")]
        public HttpResponseMessage BookMarkQuestion([FromBody] QuestionBookmark questionBookmark)
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
        #endregion

        #region "Get All Notification by user Id" 
        [HttpGet]
        [Route("api/MobileApi/GetAllNotificationByUser")]
        public HttpResponseMessage GetAllNotificationByUser(string userId)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                UserNotifications userNotifications = new UserNotifications();
                int id = Convert.ToInt32(userId);

                var userNotifications1 = (from q in db.Questions
                                          join o in db.Opinions on q.Id equals o.QuestId
                                          join n in db.Notifications on o.Id equals n.CommentId
                                          join u in db.Users on o.CommentedUserId equals u.UserID
                                          where q.OwnerUserID == id
                                          select new UserNotifications
                                          {
                                              QuestionId = q.Id,
                                              Question = q.PostQuestion,
                                              HashTags = q.HashTags,
                                              OpinionId = o.Id,
                                              Opinion = o.Comment,
                                              CommentedUserId = o.CommentedUserId,
                                              UserName = u.UserName,
                                              Like = ((n.Like ?? false) ? true : false),
                                              Dislike = ((n.Dislike ?? false) ? true : false),
                                              Comment = ((n.Comment ?? false) ? true : false)
                                          }).ToList();

                foreach (var data in userNotifications1)
                {
                    data.Message = GenerateTags(data.Like, data.Dislike, data.Comment, data.UserName);
                    data.Tag = (data.Like == true) ? "Like" : (data.Dislike == true) ? "Dislike" : (data.Comment == true) ? "Comment" : "";
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, userNotifications1, "AllOpinion"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "AllOpinion"));
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
                Tag = UserName + " Has commented on your opinion.";
            }
            else if (like == true && dislike == false && comment == true)
            {
                Tag = UserName + " Has Liked and Commented on your opinion.";
            }
            else if (dislike == true && like == false && comment == true)
            {
                Tag = UserName + " Has Disliked and Commented on your opinion.";
            }

            return Tag;
        }
        #endregion

    }
}
