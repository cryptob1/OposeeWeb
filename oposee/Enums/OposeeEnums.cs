using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace oposee.Enums
{
    public enum RecordStatus
    {
        Active,
        InActive,
        Deleted
    }
    public enum OpinionAgreeStatus
    {
        No,
        Yes
    }

    public enum ThirdPartyType
    {
        Facebook,
        Twitter,
        GooglePlus
    }
    public enum CommentStatus
    {
        DisLike,
        Like,
        RemoveLike,
        RemoveDisLike,
        Comment
    }


    public enum DeviceType
    {
        IOS,
        Android
    }

    

    public class Response
    {

        public static string Reg_InvalUserName = "Invalid User Name Min 4 Max 100 characters allowed, Regex for field is ^[a-zA-Z0-9._-]";
       


        public static string PasswordOrEmail = "The password or email you entered is not correct. Please try again.";
        public static string UserNameAlreadyExists = "User name already exists";
        public static string Invalidemail = "Invalid email";
        public static string EmailAlreadyExists = "Email already exists";
        public static string UserSettingNotFound = "User setting not found";
        public static string UserNotFound = "User not found";
        public static string CurrentPasswordNotMatched = "Current password is not matched";
        public static string RecordNotFound = "Record Not Found";
        public static string Success = "Success";
        public static string Failure = "Failure";
        public static string Available = "Available";
    }
}