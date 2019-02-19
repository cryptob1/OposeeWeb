using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace oposee.Models.API
{
    public class UserNotifications
    {
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public string HashTags { get; set; }
        public int OpinionId { get; set; }
        public string Opinion { get; set; }
        public int CommentedUserId { get; set; }
        public string UserName { get; set; }
        public bool Like { get; set; }
        public bool Dislike { get; set; }
        public bool Comment { get; set; }
        public string Tag { get; set; }
        public string Message { get; set; }
    }
}