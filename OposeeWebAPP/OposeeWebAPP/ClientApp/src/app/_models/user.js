"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var User = /** @class */ (function () {
    function User() {
    }
    return User;
}());
exports.User = User;
var Question = /** @class */ (function () {
    function Question() {
    }
    return Question;
}());
exports.Question = Question;
var LocalStorageUser = /** @class */ (function () {
    function LocalStorageUser() {
        this.Id = 0;
        this.email = '',
            this.UserName = '';
    }
    return LocalStorageUser;
}());
exports.LocalStorageUser = LocalStorageUser;
var NotificationsModel = /** @class */ (function () {
    function NotificationsModel() {
    }
    return NotificationsModel;
}());
exports.NotificationsModel = NotificationsModel;
var BookMarkQuestion = /** @class */ (function () {
    function BookMarkQuestion() {
        this.comments = [];
        this.postQuestionDetail = [new PostQuestionDetail()];
    }
    return BookMarkQuestion;
}());
exports.BookMarkQuestion = BookMarkQuestion;
var Comments = /** @class */ (function () {
    function Comments() {
    }
    return Comments;
}());
exports.Comments = Comments;
var PostQuestionDetail = /** @class */ (function () {
    function PostQuestionDetail() {
        //this.IsBookmark = false;
    }
    return PostQuestionDetail;
}());
exports.PostQuestionDetail = PostQuestionDetail;
var UserProfileModel = /** @class */ (function () {
    function UserProfileModel() {
    }
    return UserProfileModel;
}());
exports.UserProfileModel = UserProfileModel;
var UserEditProfileModel = /** @class */ (function () {
    function UserEditProfileModel() {
    }
    return UserEditProfileModel;
}());
exports.UserEditProfileModel = UserEditProfileModel;
//# sourceMappingURL=user.js.map