using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace oposee.Models.API
{
    public class PostQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int OwnerUserID { get; set; }
        public string HashTags { get; set; }
        public string CreationDate { get; set; }
        public string  ModifiedDate { get; set; }
       
    }
}