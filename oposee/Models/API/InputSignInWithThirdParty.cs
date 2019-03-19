using oposee.Enums;
using OposeeLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace oposee.Models.API
{
    public class InputSignInWithThirdParty
    {
        [Required]
        public ThirdPartyType ThirdPartyType { get; set; }

        [Required]
        public string ThirdPartyId { get; set; }
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        [RegularExpression("^[a-zA-Z0-9. _-]{0,100}$")]
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceToken { get; set; }
       

        public string ImageURL { get; set; }

        public System.DateTime CreatedDate { get; set; }
    }
    public class ImageResponse
    {
        public string ThumbnailURL { get; set; }
        public string ImageURL { get; set; }

        public string BannerImage_URL { get; set; }

        public bool IsSuccess { get; set; }
        public string ResponseMessage { get; set; }
    }
    
}