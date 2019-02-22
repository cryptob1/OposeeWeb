using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace oposee.Models.API
{
    public class PostQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int OwnerUserID { get; set; }
        public string OwnerUserName { get; set; }
        public string HashTags { get; set; }
        public string CreationDate { get; set; }
        public string ModifiedDate { get; set; }
        ////public List<UsersList> User { get; set; }
        //[Required]
        //[Display(Name = "User")]
        //public string UserId { get; set; }
        //public IEnumerable<SelectListItem> UserName { get; set; }
    }
    //public class UsersList
    //{
    //    public int Id { get; set; }
    //    public string UserName { get; set; }
    //}
}