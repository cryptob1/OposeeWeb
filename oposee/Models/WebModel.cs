using oposee.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace oposee.Models.API
{
    public class WebModel
    {
    }


    public class PostQuestionDetailWEB
    {
        public int Id { get; set; }
        public string OwnerUserName { get; set; }

        public string Question { get; set; }

        public string HashTags { get; set; }

        public int Like { get; set; }

        public string ImageURL { get; set; }

    }



    public class PostLikeDislikeModel
    {
        public int CommentId { get; set; }

        public int QuestId { get; set; }
        public string Comment { get; set; }
        public int CommentedUserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string ModifiedDate { get; set; }
        public int Likes { get; set; }
        public bool OpinionAgreeStatus { get; set; }
        public int Dislikes { get; set; }

        public bool LikeOrDislke { get; set; }



    }


    public class PagingModel
    {
        public int UserId { get; set; }

        public string Search { get; set; }

        public int PageNumber { get; set; }

        public int TotalRecords { get; set; }

        public int PageSize { get; set; }


    }

    public class PostQuestionDetailWebModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int OwnerUserID { get; set; }

        public string OwnerUserName { get; set; }
        public string Name { get; set; }
        public string UserImage { get; set; }
        public string HashTags { get; set; }
        public int? TotalLikes { get; set; }
        public int? TotalDisLikes { get; set; }
        public DateTime? CreationDate { get; set; }
        public Comments MostYesLiked { get; set; }
        public Comments MostNoLiked { get; set; }

        public int YesCount { get; set; }
        public int NoCount { get; set; }

        public int TotalRecordcount { get; set; }


    }


    public class QuestionBookmarkWebModel
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public bool IsBookmark { get; set; }
        public int UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class AllUserQuestionsWebModel
    {
        public List<PostQuestionDetailWebModel> PostQuestionDetail { get; set; }
        //public List<Comments> Comments { get; set; }

    }
    public class InputSignInWithThirdPartyWebModel
    {

        public ThirdPartyType ThirdPartyType { get; set; }
        public string ThirdPartyId { get; set; }
        public string Email { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceToken { get; set; }


        public string ImageURL { get; set; }

        public System.DateTime CreatedDate { get; set; }


    }
    public class PostQuestionModel
    {

        public int Id { get; set; }
        public string PostQuestion { get; set; }
        public int OwnerUserID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string TaggedUser { get; set; }
        public string HashTags { get; set; }
    }


}