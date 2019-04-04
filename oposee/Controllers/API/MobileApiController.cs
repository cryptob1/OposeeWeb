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
using oposee.Models;
using System.Web;
using MvcPaging;
using OposeeLibrary.PushNotfication;
using System.Configuration;

namespace oposee.Controllers.API
{
    [RoutePrefix("oposee")]
    public class MobileApiController : ApiController
    {
        oposeeDbEntities db = new oposeeDbEntities();
        public static string con = ConfigurationManager.ConnectionStrings["oposeeDbEntitiesSp"].ToString();
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
                    entity.ImageURL = entity.ImageURL;
                    //if (input.ImageURL != null && input.ImageURL != "")
                    //{
                    //    try
                    //    {
                    //        string strTempImageSave = OposeeLibrary.Utilities.ResizeImage.Download_Image(input.ImageURL);
                    //        string profileFilePath = _SiteURL + "/ProfileImage/" + strTempImageSave;
                    //        strIamgeURLfordb = profileFilePath;
                    //        entity.ImageURL = profileFilePath;
                    //    }
                    //    catch (Exception ex)
                    //    {

                    //    }
                    //}
                    //else
                    //{
                    //    entity.ImageURL = _SiteURL + "/ProfileImage/oposee-profile.png";
                    //}
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
                    else
                    {

                        entity.ImageURL = _SiteURL + "/ProfileImage/oposee-profile.png";
                        strIamgeURLfordb = entity.ImageURL;
                    }
                    entity.ImageURL = strIamgeURLfordb;
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
        #endregion

        #region "Post Question" 
        [HttpPost]
        [Route("api/MobileApi/PostQuestion")]
        public HttpResponseMessage PostQuestion([FromBody] Question postQuestion)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                Question quest = null;
                PushNotifications pushNotifications = new PushNotifications();
                quest = db.Questions.Where(p => p.Id == postQuestion.Id
                                       ).FirstOrDefault();
                if (quest != null)
                {
                    quest.PostQuestion = postQuestion.PostQuestion;
                    quest.OwnerUserID = postQuestion.OwnerUserID;
                    quest.HashTags = postQuestion.HashTags;
                    quest.TaggedUser = postQuestion.TaggedUser;
                    quest.IsDeleted = false;
                    quest.ModifiedDate = DateTime.Now;
                    db.Entry(quest).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    int questID = quest.Id;
                    quest = db.Questions.Find(questID);
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, quest, "Question"));
                }
                else
                {
                    quest = new Question();
                    Token token = new Token();
                    quest.PostQuestion = postQuestion.PostQuestion;
                    quest.OwnerUserID = postQuestion.OwnerUserID;
                    quest.HashTags = postQuestion.HashTags;
                    quest.TaggedUser = postQuestion.TaggedUser;
                    quest.IsDeleted = false;
                    quest.CreationDate = DateTime.Now;
                    db.Questions.Add(quest);
                    db.SaveChanges();
                    token = db.Tokens.Where(p => p.UserId == postQuestion.OwnerUserID).FirstOrDefault();
                    token.BalanceToken = token.BalanceToken - 1;

                    db.Entry(token).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    User questionOwner = db.Users.Find(quest.OwnerUserID);
                    int questID = quest.Id;
                    quest = db.Questions.Find(questID);
                    string taggedUser = postQuestion.TaggedUser;

                    if (!string.IsNullOrEmpty(taggedUser))
                    {
                        var roleIds = taggedUser.Split(',').Select(s => int.Parse(s));
                        foreach (int items in roleIds)
                        {
                            User data = db.Users.Find(items);
                            if (data != null)
                            {
                                string finalMessage = questionOwner.FirstName + " " + questionOwner.LastName + " has tagged you in question";

                                pushNotifications.SendNotification_Android(data.DeviceToken, finalMessage, "QD", questID.ToString());
                            }
                        }
                    }

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
                    int opinionID = opinion.Id;
                    opinion = db.Opinions.Find(opinionID);
                    notification = db.Notifications.Where(p => p.CommentedUserId == postAnswer.CommentedUserId && p.CommentId == opinionID).FirstOrDefault();
                    if (notification != null)
                    {
                        notification.CommentedUserId = postAnswer.CommentedUserId;
                        notification.CommentId = opinionID;
                        notification.questId = postAnswer.QuestId;
                        notification.ModifiedDate = DateTime.Now;
                        db.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        notification = new Notification();
                        notification.CommentedUserId = postAnswer.CommentedUserId;
                        notification.CommentId = opinionID;
                        notification.questId = postAnswer.QuestId;
                        notification.CreationDate = DateTime.Now;
                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, opinion, "Opinion"));
                }
                else
                {
                    PushNotifications pushNotifications = new PushNotifications();
                    opinion = new Opinion();
                    Token token = new Token();
                    opinion.Comment = postAnswer.Comment;
                    opinion.QuestId = postAnswer.QuestId;
                    opinion.CommentedUserId = postAnswer.CommentedUserId;
                    opinion.IsAgree = postAnswer.OpinionAgreeStatus;

                    opinion.CreationDate = DateTime.Now;
                    db.Opinions.Add(opinion);
                    db.SaveChanges();
                    token = db.Tokens.Where(p => p.UserId == postAnswer.CommentedUserId).FirstOrDefault();
                    token.BalanceToken = token.BalanceToken - 1;

                    db.Entry(token).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    int opinionID = opinion.Id;
                    opinion = db.Opinions.Find(opinionID);
                    notification = db.Notifications.Where(p => p.CommentedUserId == postAnswer.CommentedUserId && p.CommentId == opinionID).FirstOrDefault();
                    Question quest = db.Questions.Find(postAnswer.QuestId);
                    User questOwner = db.Users.Where(u => u.UserID == quest.OwnerUserID).FirstOrDefault();
                    User user = db.Users.Where(u => u.UserID == postAnswer.CommentedUserId).FirstOrDefault();
                    if (questOwner != null)
                    {
                        if (quest.OwnerUserID != postAnswer.CommentedUserId)
                        {
                            //***** Notification to question owner
                            string finalMessage = GenerateTagsForQuestion(false, false, true, user.FirstName + " " + user.LastName);

                            pushNotifications.SendNotification_Android(questOwner.DeviceToken, finalMessage, "QD", postAnswer.QuestId.ToString());
                            //***** Notification to Tagged Users
                            string taggedUser = quest.TaggedUser;

                            if (!string.IsNullOrEmpty(taggedUser))
                            {
                                var roleIds = taggedUser.Split(',').Select(s => int.Parse(s));
                                foreach (int items in roleIds)
                                {
                                    if (postAnswer.CommentedUserId != items)
                                    {
                                        User data = db.Users.Find(items);
                                        if (data != null)
                                        {
                                            string finalMessage1 = user.FirstName + " " + user.LastName + " has given opinion on question in which you're tagged in.";

                                            pushNotifications.SendNotification_Android(data.DeviceToken, finalMessage1, "QD", postAnswer.QuestId.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        else if (quest.OwnerUserID == postAnswer.CommentedUserId)
                        {
                            //in this block notification will send to tagged users
                            string taggedUser = quest.TaggedUser;

                            if (!string.IsNullOrEmpty(taggedUser))
                            {
                                var roleIds = taggedUser.Split(',').Select(s => int.Parse(s));
                                foreach (int items in roleIds)
                                {
                                    User data = db.Users.Find(items);
                                    if (data != null)
                                    {
                                        string finalMessage = user.FirstName + " " + user.LastName + " has given opinion on question in which you're tagged in.";

                                        pushNotifications.SendNotification_Android(data.DeviceToken, finalMessage, "QD", postAnswer.QuestId.ToString());
                                    }
                                }
                            }
                        }
                    }

                    if (notification != null)
                    {
                        notification.CommentedUserId = postAnswer.CommentedUserId;
                        notification.CommentId = opinionID;
                        notification.questId = postAnswer.QuestId;
                        notification.ModifiedDate = DateTime.Now;
                        db.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        notification = new Notification();
                        notification.CommentedUserId = postAnswer.CommentedUserId;
                        notification.CommentId = opinionID;
                        notification.Comment = true;
                        notification.questId = postAnswer.QuestId;
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

        public string GenerateTagsForQuestion(bool? like, bool? dislike, bool? comment, string UserName)
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
        public string GenerateTagsForOpinion(bool? like, bool? dislike, bool? comment, string UserName)
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

        public string GenerateTagsForTaggedUsers(bool? like, bool? dislike, bool? comment, string ActionUserName)
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

        #region "Get All Opinion by question Id" 
        [HttpGet]
        [Route("api/MobileApi/GetAllOpinion")]
        public HttpResponseMessage GetAllOpinion(string questId, string userid, int Pageindex, int Pagesize)
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
                    //                  title == null ?
                    //items.Where(x => x.Title == null) : items.Where(x => x.Title == title);
                    string taggedUser = db.Questions.Find(id).TaggedUser;
                    IEnumerable<int> roleIds = null;
                    if (!string.IsNullOrEmpty(taggedUser))
                    {
                        roleIds = db.Questions.Find(id).TaggedUser.Split(',').Select(s => int.Parse(s));
                    }
                    else
                    {
                        roleIds = new int[] { };
                    }
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
                                                             Name = u.FirstName + " " + u.LastName,
                                                             IsBookmark = db.BookMarks.Where(b => b.UserId == userId && b.QuestionId == id).Select(b => b.IsBookmark.HasValue ? b.IsBookmark.Value : false).FirstOrDefault(),

                                                             IsUserPosted = db.Opinions.Any(cus => cus.CommentedUserId == userId && cus.QuestId == id),
                                                             TotalLikes = db.Notifications.Where(o => o.questId == q.Id && o.Like == true).Count(),
                                                             TotalDisLikes = db.Notifications.Where(o => o.questId == q.Id && o.Dislike == true).Count(),
                                                             TaggedUsers = db.Users.Where(k => roleIds.Contains(k.UserID)).Select(p => p.FirstName + " " + p.LastName).AsQueryable(),
                                                             YesCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == true).Count(),
                                                             NoCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == false).Count(),
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
                                                   Name = t.FirstName + " " + t.LastName,
                                                   UserImage = string.IsNullOrEmpty(t.ImageURL) ? "" : t.ImageURL,
                                                   LikesCount = db.Notifications.Where(p => p.CommentId == e.Id && p.Like == true).Count(),
                                                   DislikesCount = db.Notifications.Where(p => p.CommentId == e.Id && p.Dislike == true).Count(),
                                                   Likes = db.Notifications.Where(p => p.CommentedUserId == userId && p.CommentId == e.Id).Select(b => b.Like.HasValue ? b.Like.Value : false).FirstOrDefault(),
                                                   DisLikes = db.Notifications.Where(p => p.CommentedUserId == userId && p.CommentId == e.Id).Select(b => b.Dislike.HasValue ? b.Dislike.Value : false).FirstOrDefault(),
                                                   CommentedUserName = t.UserName,
                                                   IsAgree = e.IsAgree,
                                                   CreationDate = e.CreationDate
                                               }).OrderByDescending(p => p.CreationDate).ToPagedList(Pageindex - 1, Pagesize).ToList();

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
        public HttpResponseMessage GetAllPostsByUserId(string UserID, int Pageindex, int Pagesize)
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
                                                     where u.UserID == id && q.IsDeleted == false
                                                     select new PostQuestionDetail
                                                     {
                                                         Id = q.Id,
                                                         Question = q.PostQuestion,
                                                         OwnerUserID = q.OwnerUserID,
                                                         OwnerUserName = u.UserName,
                                                         Name = u.FirstName + " " + u.LastName,
                                                         UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                                         HashTags = q.HashTags,
                                                         TotalLikes = db.Notifications.Where(o => o.questId == q.Id && o.Like == true).Count(),
                                                         TotalDisLikes = db.Notifications.Where(o => o.questId == q.Id && o.Dislike == true).Count(),
                                                         YesCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == true).Count(),
                                                         NoCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == false).Count(),
                                                         CreationDate = q.CreationDate
                                                     }).OrderByDescending(p => p.CreationDate).ToPagedList(Pageindex - 1, Pagesize).ToList();


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
        public HttpResponseMessage GetAllPosts(int Pageindex, int Pagesize)
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
                                                     where q.IsDeleted == false
                                                     select new PostQuestionDetail
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
                                                     }).OrderByDescending(p => p.CreationDate).ToPagedList(Pageindex - 1, Pagesize).ToList();


                foreach (var data in questionDetail.PostQuestionDetail)
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
                string action = "";
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                Notification notification = null;
                PushNotifications pNoty = new PushNotifications();
                notification = db.Notifications.Where(p => p.CommentedUserId == likeDislike.CommentedUserId && p.CommentId == likeDislike.CommentId).FirstOrDefault();
                if (notification != null)
                {
                    if (likeDislike.CommentStatus == CommentStatus.DisLike)
                    {
                        notification.Dislike = true;
                        notification.Like = false;
                        action = "dislike";
                    }
                    else if (likeDislike.CommentStatus == CommentStatus.Like)
                    {
                        notification.Like = true;
                        notification.Dislike = false;
                        action = "like";
                    }
                    if (likeDislike.CommentStatus == CommentStatus.RemoveLike)
                    {
                        notification.Like = false;
                        action = "remove like";
                    }
                    else if (likeDislike.CommentStatus == CommentStatus.RemoveDisLike)
                    {
                        notification.Dislike = false;
                        action = "remove dislike";
                    }
                    notification.CommentedUserId = likeDislike.CommentedUserId;
                    notification.CommentId = likeDislike.CommentId;
                    notification.questId = likeDislike.questId;
                    notification.ModifiedDate = DateTime.Now;
                    db.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    List<Opinion> opinion = db.Opinions.Where(p => p.Id == likeDislike.CommentId).ToList();
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
                            string finalMessage = GenerateTagsForQuestion(notification.Like, notification.Dislike, false, user.FirstName + " " + user.LastName);

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
                            string finalMessage = GenerateTagsForOpinion(notification.Like, notification.Dislike, false, user.FirstName + " " + user.LastName);

                            pNoty.SendNotification_Android(commentOwner.DeviceToken, finalMessage, "QD", questId.ToString());
                        }
                    }
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

                    PushNotifications pushNotifications = new PushNotifications();

                    if (likeDislike.CommentStatus == CommentStatus.DisLike)
                    {
                        notification.Dislike = true;
                        notification.Like = false;
                        action = "dislike";
                    }
                    else if (likeDislike.CommentStatus == CommentStatus.Like)
                    {
                        notification.Like = true;
                        notification.Dislike = false;
                        action = "like";
                    }
                    if (likeDislike.CommentStatus == CommentStatus.RemoveLike)
                    {
                        notification.Like = false;
                        action = "remove like";
                    }
                    else if (likeDislike.CommentStatus == CommentStatus.RemoveDisLike)
                    {
                        notification.Dislike = false;
                        action = "remove disLike";
                    }
                    notification.CommentedUserId = likeDislike.CommentedUserId;
                    notification.CommentId = likeDislike.CommentId;
                    notification.questId = likeDislike.questId;
                    notification.CreationDate = DateTime.Now;

                    db.Notifications.Add(notification);
                    db.SaveChanges();
                    int questID = notification.Id;
                    notification = db.Notifications.Find(questID);

                    List<Opinion> op = db.Opinions.Where(p => p.Id == likeDislike.CommentId).ToList();
                    int questId = op[0].QuestId;
                    Question ques = db.Questions.Where(p => p.Id == questId).FirstOrDefault();
                    User questOwner = db.Users.Where(u => u.UserID == ques.OwnerUserID).FirstOrDefault();
                    int OpinionUserID = op[0].CommentedUserId;
                    User commentOwner = db.Users.Where(u => u.UserID == OpinionUserID).FirstOrDefault();

                    User user = db.Users.Where(u => u.UserID == notification.CommentedUserId).FirstOrDefault();
                    //if (questOwner != null)
                    //{
                    //    string finalMessage = GenerateTagsForQuestion(notification.Like, notification.Dislike, false, user.FirstName + " " + user.LastName);

                    //    pNoty.SendNotification_Android(questOwner.DeviceToken, finalMessage, "QD", questId.ToString());
                    //    //updateCount = true;
                    //}
                    if (questOwner != null && (!action.Contains("remove")))
                    {
                        if (ques.OwnerUserID != notification.CommentedUserId)
                        {
                            //***** Notification to question owner
                            string finalMessage = GenerateTagsForQuestion(notification.Like, notification.Dislike, false, user.FirstName + " " + user.LastName);

                            pushNotifications.SendNotification_Android(questOwner.DeviceToken, finalMessage, "QD", questId.ToString());
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

                                            pushNotifications.SendNotification_Android(data.DeviceToken, finalMessage1, "QD", questId.ToString());
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

                                        pushNotifications.SendNotification_Android(data.DeviceToken, finalMessage, "QD", questId.ToString());
                                    }
                                }
                            }
                        }
                        if (commentOwner.UserID != notification.CommentedUserId)
                        {
                            //***** Notification to question owner
                            string finalMessage = GenerateTagsForOpinion(notification.Like, notification.Dislike, false, user.FirstName + " " + user.LastName);

                            pushNotifications.SendNotification_Android(commentOwner.DeviceToken, finalMessage, "QD", questId.ToString());
                        }
                    }
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
        public HttpResponseMessage SearchByHashTagOrString(string search, int Pageindex, int Pagesize)
        {
            try
            {

                if (search.Contains("~"))
                {
                    search = search.Replace("~", "#").ToString();
                }
                var result = (from q in db.Questions
                              join u in db.Users on q.OwnerUserID equals u.UserID
                              where q.IsDeleted == false
                              select new PostQuestionDetail
                              {
                                  Id = q.Id,
                                  Question = q.PostQuestion,
                                  OwnerUserID = q.OwnerUserID,
                                  OwnerUserName = u.UserName,
                                  Name = u.FirstName + " " + u.LastName,
                                  UserImage = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                                  HashTags = q.HashTags,
                                  CreationDate = q.CreationDate,
                                  TotalLikes = db.Notifications.Where(o => o.questId == q.Id && o.Like == true).Count(),
                                  TotalDisLikes = db.Notifications.Where(o => o.questId == q.Id && o.Dislike == true).Count(),
                                  YesCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == true).Count(),
                                  NoCount = db.Opinions.Where(o => o.QuestId == q.Id && o.IsAgree == false).Count()
                              }).Where(s => s.HashTags.Contains(search) || s.Question.Contains(search)).OrderByDescending(p => p.CreationDate).ToPagedList(Pageindex - 1, Pagesize).ToList();

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
        public HttpResponseMessage GetAllNotificationByUser(string userId, int Pageindex, int Pagesize)
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
                                          where q.OwnerUserID == id && q.IsDeleted == false
                                          select new UserNotifications
                                          {
                                              QuestionId = q.Id,
                                              Question = q.PostQuestion,
                                              HashTags = q.HashTags,
                                              OpinionId = o.Id,
                                              Opinion = o.Comment,
                                              Image = u.ImageURL,
                                              Name = u.FirstName + " " + u.LastName,
                                              CommentedUserId = o.CommentedUserId,
                                              UserName = u.UserName,
                                              Like = ((n.Like ?? false) ? true : false),
                                              Dislike = ((n.Dislike ?? false) ? true : false),
                                              Comment = ((n.Comment ?? false) ? true : false),
                                              CreationDate = n.CreationDate,
                                              ModifiedDate = n.ModifiedDate
                                          }).OrderByDescending(p => p.CreationDate).ToPagedList(Pageindex - 1, Pagesize).ToList();

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
        #endregion

        #region "Get User Profile" 
        [HttpGet]
        [Route("api/MobileApi/GetUserProfile")]
        public HttpResponseMessage GetUserProfile(int userid)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                UserProfile UserProfile = new UserProfile();
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


                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, UserProfile, "UserProfile"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "UserProfile"));
            }
        }
        #endregion


        #region "Update User Profile" 
        [HttpPost]
        [Route("api/MobileApi/UpdateUserProfile")]
        public HttpResponseMessage UpdateUserProfile()
        {
            try
            {
                var httpContext = (HttpContextWrapper)Request.Properties["MS_HttpContext"];
                int userid = 0;
                string firstname = "";
                string lastname = "";
                if (httpContext.Request.Form["UserID"] == "")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Info, "provide UserID"));
                }
                else
                {
                    userid = Convert.ToInt32(httpContext.Request.Form["UserID"].ToString());
                }
                if (httpContext.Request.Form["FirstName"] != "")
                {
                    firstname = httpContext.Request.Form["FirstName"].ToString();

                }
                if (httpContext.Request.Form["LastName"] != "")
                {
                    lastname = httpContext.Request.Form["LastName"].ToString();

                }
                //ViewModelUser User = new ViewModelUser();
                string strIamgeURLfordb = null;
                string _SiteRoot = WebConfigurationManager.AppSettings["SiteImgPath"];
                string _SiteURL = WebConfigurationManager.AppSettings["SiteImgURL"];
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                User entity = null;
                entity = db.Users.Find(userid);

                if (entity != null)
                {
                    //entity.ImageURL = "";
                    entity.FirstName = firstname;
                    entity.LastName = lastname;
                    entity.ModifiedDate = DateTime.Now;
                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        ImageResponse imgResponse = MultipartFiles.GetMultipartImage(HttpContext.Current.Request.Files, "Image", "profileimage", _Imagethumbsize, _Imagethumbsize, _imageSize, _imageSize, true, true, true, "profileimage");

                        if (imgResponse.IsSuccess == true)
                        {
                            //entity.ThumbnailURL = imgResponse.ThumbnailURL;
                            entity.ImageURL = imgResponse.ImageURL;
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Info, imgResponse.ResponseMessage));
                        }
                        // _UsersRepo.Repository.Update(entity);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(entity.ImageURL))
                        {
                            entity.ImageURL = _SiteURL + "/ProfileImage/oposee-profile.png";
                        }
                        //return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Info, "Image file is not provided", "UserData"));
                    }
                    //if (User.ImageURL != null && User.ImageURL != "")
                    //{
                    //    //try
                    //    //{
                    //    //    string strTempImageSave = OposeeLibrary.Utilities.ResizeImage.Download_Image(User.ImageURL);
                    //    //    string profileFilePath = _SiteURL + "/ProfileImage/" + strTempImageSave;
                    //    //    strIamgeURLfordb = profileFilePath;
                    //    //    entity.ImageURL = profileFilePath;
                    //    //}
                    //    //catch (Exception ex)
                    //    //{

                    //    //}
                    //}
                    //else
                    //{
                    //    entity.ImageURL = _SiteURL + "/ProfileImage/oposee-profile.png";
                    //}
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    int userID = entity.UserID;
                    entity = db.Users.Find(userID);
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, entity, "UpdateUserProfile"));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, "User Not Found", "UpdateUserProfile"));
                }

            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "UpdateUserProfile"));
            }
        }
        #endregion

        #region "Delete Question" 
        [HttpPost]
        [Route("api/MobileApi/DeleteQuestion")]
        public HttpResponseMessage DeleteQuestion(int questId)
        {
            try
            {
                Question question = db.Questions.Find(questId);

                if (question != null)
                {
                    //db.Questions.Remove(question);
                    //db.SaveChanges();
                    question.IsDeleted = true;
                    db.Entry(question).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, question, "DeleteQuestion"));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, "Question Not Found", "DeleteQuestion"));
                }

            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "DeleteQuestion"));
            }
        }
        #endregion


        #region "Get All BookMark By Id" 
        [HttpGet]
        [Route("api/MobileApi/GetAllBookMarkById")]
        public HttpResponseMessage GetAllBookMarkById(int userId, int Pageindex, int Pagesize)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.OK, ModelState);
                }
                AllUserQuestions questionDetail = new AllUserQuestions();
                //int id = Convert.ToInt32(userId);

                questionDetail.PostQuestionDetail = (from q in db.Questions
                                                     join b in db.BookMarks on q.Id equals b.QuestionId
                                                     join u in db.Users on b.UserId equals u.UserID
                                                     where q.IsDeleted == false && u.UserID == userId && b.IsBookmark == true
                                                     select new PostQuestionDetail
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
                                                     }).OrderByDescending(p => p.CreationDate).ToPagedList(Pageindex - 1, Pagesize).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, questionDetail, "GetBookmarkQuestion"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "GetBookmarkQuestion"));
            }
        }
        #endregion


        #region "Get All Users" 
        [HttpGet]
        [Route("api/MobileApi/GetAllUsers")]
        public HttpResponseMessage GetAllUsers(int Pageindex, int Pagesize)
        {
            try
            {
                List<ViewModelUser> user = new List<ViewModelUser>();

                user = (from u in db.Users
                        where u.IsAdmin != true
                        select new ViewModelUser
                        {
                            UserID = u.UserID,
                            UserName = u.UserName,
                            Name = u.FirstName + " " + u.LastName,
                            ImageURL = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                            CreatedDate = u.CreatedDate
                        }).OrderByDescending(p => p.CreatedDate).ToPagedList(Pageindex - 1, Pagesize).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, user, "GetAllUsers"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "GetAllUsers"));
            }
        }
        #endregion


        #region "Get All Search Users" 
        [HttpGet]
        [Route("api/MobileApi/GetAllSearchUsers")]
        public HttpResponseMessage GetAllSearchUsers(string search)
        {
            try
            {
                List<ViewModelUser> user = new List<ViewModelUser>();

                user = (from u in db.Users
                        where u.IsAdmin != true
                        select new ViewModelUser
                        {
                            UserID = u.UserID,
                            UserName = u.UserName,
                            Name = u.FirstName + " " + u.LastName,
                            ImageURL = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
                            CreatedDate = u.CreatedDate
                        }).Where(s => s.UserName.Contains(search) || s.Name.Contains(search)).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, user, "GetAllUsers"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "GetAllUsers"));
            }
        }
        #endregion

        //#region "Get All Search Users" 
        //[HttpGet]
        //[Route("api/MobileApi/GetAllTaggedUsers")]
        //public HttpResponseMessage GetAllTaggedUsers(string questId)
        //{
        //    try
        //    {
        //        List<ViewTaggedUser> user = new List<ViewTaggedUser>();
        //        int id = Convert.ToInt32(questId);
        //        Question quest = db.Questions.Find(id);
        //        user = (from u in db.Users
        //                where quest.TaggedUser.Contains(u.UserID.ToString())
        //                select new ViewTaggedUser
        //                {
        //                    UserID = u.UserID.ToString(),
        //                    UserName = u.UserName,
        //                    Name = u.FirstName + " " + u.LastName,
        //                    FirstName = u.FirstName,
        //                    LastName = u.LastName,
        //                    TaggedUser = quest.TaggedUser,
        //                    ImageURL = string.IsNullOrEmpty(u.ImageURL) ? "" : u.ImageURL,
        //                    CreatedDate = u.CreatedDate
        //                }).ToList();
        //        return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, user, "GetAllUsers"));
        //    }
        //    catch (Exception ex)
        //    {
        //        OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
        //        return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "GetAllUsers"));
        //    }
        //}
        //#endregion
        #region "Get All Search Users" 
        [HttpGet]
        [Route("api/MobileApi/GetAllTaggedUsers")]
        public HttpResponseMessage GetAllTaggedUsers(int questId)
        {
            try
            {
                List<ViewTaggedUser> user = new List<ViewTaggedUser>();
                int id = Convert.ToInt32(questId);

                Question questTags = db.Questions.Where(x => x.Id == questId).FirstOrDefault();

                if (!string.IsNullOrEmpty(questTags.TaggedUser))
                {
                    string TagIds = questTags.TaggedUser.ToString();
                    var stringsarray = TagIds.Split(',');
                    string stringFinal = "";
                    foreach (var item in stringsarray)
                    {
                        stringFinal += "'" + item + "',";
                    }

                    stringFinal = stringFinal.Substring(0, stringFinal.Length - 1);
                    string query = "select * from users where CAST(userid as int)  in (" + stringFinal + ")";

                    //List<ViewTaggedUser> user1 = new List<ViewTaggedUser>();
                    SqlConnection connection = new SqlConnection(con);
                    var command1 = new SqlCommand(query, connection);
                    command1.CommandType = System.Data.CommandType.Text;
                    connection.Open();
                    SqlDataReader reader = command1.ExecuteReader();
                    ViewTaggedUser objitem = null;
                    while (reader.Read())
                    {
                        objitem = new ViewTaggedUser();
                        objitem.UserID = reader["UserID"].ToString();
                        objitem.UserName = reader["UserName"].ToString();
                        objitem.FirstName = reader["FirstName"].ToString();
                        objitem.LastName = reader["LastName"].ToString();
                        objitem.TaggedUser = TagIds;
                        objitem.UserID = reader["UserID"].ToString();
                        objitem.ImageURL = string.IsNullOrEmpty(reader["ImageURL"].ToString()) ? "" : reader["ImageURL"].ToString();
                        objitem.Name = objitem.FirstName + objitem.LastName;
                        user.Add(objitem);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, user, "GetAllUsers"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "GetAllUsers"));
            }
        }
        #endregion

        #region "Get Other User Profile" 
        [HttpGet]
        [Route("api/MobileApi/GetOtherUserProfile")]
        public HttpResponseMessage GetOtherUserProfile(int userid, int Pageindex, int Pagesize)
        {
            try
            {
                OtherUserProfile UserProfile = new OtherUserProfile();
                UserProfile.UserProfile = (from q in db.Questions
                                           join u in db.Users on q.OwnerUserID equals u.UserID
                                           where u.UserID == userid
                                           select new OtherUsers
                                           {
                                               UserID = u.UserID,
                                               UserName = u.UserName,
                                               FirstName = u.FirstName,
                                               LastName = u.LastName,
                                               Name = u.FirstName + " " + u.LastName,
                                               Email = u.Email,
                                               ImageURL = u.ImageURL,
                                               TotalPostedQuestion = db.Questions.Where(p => p.OwnerUserID == userid && p.IsDeleted == false).Count()
                                           }).FirstOrDefault();


                UserProfile.Question = (from q in db.Questions
                                        join u in db.Users on q.OwnerUserID equals u.UserID
                                        where u.UserID == userid
                                        select new PostQuestionDetail
                                        {
                                            Id = q.Id,
                                            Question = q.PostQuestion,
                                            TotalLikes = (from q in db.Questions
                                                          join o in db.Opinions on q.Id equals o.QuestId
                                                          where q.OwnerUserID == userid && q.IsDeleted == false
                                                          select o.Likes).Sum(),
                                            TotalDisLikes = (from q in db.Questions
                                                             join o in db.Opinions on q.Id equals o.QuestId
                                                             where q.OwnerUserID == userid && q.IsDeleted == false
                                                             select o.Dislikes).Sum(),
                                            CreationDate = q.CreationDate
                                        }).OrderByDescending(p => p.CreationDate).ToPagedList(Pageindex - 1, Pagesize).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Success, UserProfile, "OtherUserProfile"));
            }
            catch (Exception ex)
            {
                OposeeLibrary.Utilities.LogHelper.CreateLog3(ex, Request);
                return Request.CreateResponse(HttpStatusCode.OK, JsonResponse.GetResponse(ResponseCode.Failure, ex.Message, "OtherUserProfile"));
            }
        }
        #endregion


    }
}
