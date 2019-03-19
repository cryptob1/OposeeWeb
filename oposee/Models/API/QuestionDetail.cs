using oposee.Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace oposee.Models.API
{
    public class QuestionDetail
    {
        public PostQuestionDetail PostQuestionDetail { get; set; }
        public List<Comments> Comments { get; set; }

    }
    public class BookMarkQuestion
    {
        public BookMarkQuestionDetail PostQuestionDetail { get; set; }
        public List<Comments> Comments { get; set; }

    }
    public class AllUserQuestions
    {
        public List<PostQuestionDetail> PostQuestionDetail { get; set; }
        //public List<Comments> Comments { get; set; }

    }
    public class PostQuestionDetail
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int OwnerUserID { get; set; }
        [Display(Name = "UserName")]
        public string OwnerUserName { get; set; }
        public string Name { get; set; }
        public string UserImage { get; set; }
        public string HashTags { get; set; }
        public int? TotalLikes { get; set; }
        public int YesCount { get; set; }
        public int NoCount { get; set; }
        public int? TotalDisLikes { get; set; }
        public DateTime? CreationDate { get; set; }
        public Comments MostYesLiked { get; set; }
        public Comments MostNoLiked { get; set; }

    }
    public class BookMarkQuestionDetail
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int OwnerUserID { get; set; }
        [Display(Name = "UserName")]
        public string OwnerUserName { get; set; }
        public string UserImage { get; set; }
        public string Name { get; set; }
        public string HashTags { get; set; }
        public int? TotalLikes { get; set; }
        public int? TotalDisLikes { get; set; }
        public int YesCount { get; set; }
        public int NoCount { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsBookmark { get; set; }
        public int BookmarkId { get; set; }
        public bool IsUserPosted { get; set; }
        public IQueryable TaggedUsers { get; set; }
    }
    
    public class Comments
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int CommentedUserId { get; set; }
        public string CommentedUserName { get; set; }
        public int LikesCount { get; set; }
        public string Name { get; set; }
        public string UserImage { get; set; }
        public int DislikesCount { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool Likes { get; set; }
        public bool DisLikes { get; set; }
        public bool? IsAgree { get; set; }


    }
}