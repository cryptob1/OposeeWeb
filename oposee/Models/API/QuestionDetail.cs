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
        public string UserImage { get; set; }
        public string HashTags { get; set; }
        public DateTime? CreationDate { get; set; }

    }
    public class Comments
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int CommentedUserId { get; set; }
        public string CommentedUserName { get; set; }
        public int Likes { get; set; }
        public string UserImage { get; set; }
        public int Dislikes { get; set; }
        public DateTime? CreationDate { get; set; }

    }
}