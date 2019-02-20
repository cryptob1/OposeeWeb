using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace oposee.Models
{
    public class ViewModelUser
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SocialID { get; set; }
        public string SocialType { get; set; }
        public string ImageURL { get; set; }
        public HttpPostedFileBase ImageURL_data { get; set; }
        public string DeviceType { get; set; }
        public string DeviceToken { get; set; }
        public string RecordStatus { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}