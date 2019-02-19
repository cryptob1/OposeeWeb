using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace oposee.Models.API
{
    public class QuestionBookmark
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public bool IsBookmark { get; set; }
        public int UserId { get; set; }
        public string CreationDate { get; set; }
        public string ModifiedDate { get; set; }
    }
}