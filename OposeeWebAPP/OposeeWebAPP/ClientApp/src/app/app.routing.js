"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var router_1 = require("@angular/router");
var index_1 = require("./home/index");
var index_2 = require("./login/index");
var index_3 = require("./register/index");
var index_4 = require("./_guards/index");
var PostQuestion_1 = require("./question/PostQuestion");
var QuestionListing_1 = require("./question/QuestionListing");
var notification_component_1 = require("./notification/notification.component");
var questiondetail_component_1 = require("./questionDetail/questiondetail.component");
var profile_component_1 = require("./user/profile/profile.component");
var editProfile_component_1 = require("./user/editprofile/editProfile.component");
var userpostQuestion_component_1 = require("./user/userpostedQuestion/userpostQuestion.component");
var bookmark_component_1 = require("./bookmark/bookmark.component");
var postedQuestionEditList_component_1 = require("./question/PostedQuestion/postedQuestionEditList.component");
var editPostquestion_component_1 = require("./question/editPosttedQuestion/editPostquestion.component");
var termandCondition_component_1 = require("./legal/termandCondition.component");
var privatePolicy_component_1 = require("./legal/privatePolicy.component");
var appRoutes = [
    { path: 'questionlisting/:search', component: QuestionListing_1.QuestionListingComponent },
    { path: '', component: index_1.HomeComponent },
    { path: 'login', component: index_2.LoginComponent },
    { path: 'register', component: index_3.RegisterComponent },
    { path: 'postquestion/:questionId', component: PostQuestion_1.PostQuestionComponent, canActivate: [index_4.AuthGuard] },
    { path: 'notifications', component: notification_component_1.NotificationComponent, canActivate: [index_4.AuthGuard] },
    // otherwise redirect to home
    { path: 'questiondetail/:Id', component: questiondetail_component_1.Questiondetail, canActivate: [index_4.AuthGuard] },
    { path: 'profile/:Id', component: profile_component_1.ProfileComponent, canActivate: [index_4.AuthGuard] },
    { path: 'editprofile/:Id', component: editProfile_component_1.EditProfileComponent, canActivate: [index_4.AuthGuard] },
    { path: 'mypostquestions', component: userpostQuestion_component_1.UserpostQuestion, canActivate: [index_4.AuthGuard] },
    { path: 'bookmark/:questionId', component: bookmark_component_1.BookmarkQuestion, canActivate: [index_4.AuthGuard] },
    { path: 'postedQuestionEditList/:questionId', component: postedQuestionEditList_component_1.PostedQuestionEditList, canActivate: [index_4.AuthGuard] },
    { path: 'editpostedquestion/:qId', component: editPostquestion_component_1.EditPostquestion, canActivate: [index_4.AuthGuard] },
    { path: 'termcondition', component: termandCondition_component_1.termandConditionComponent, canActivate: [index_4.AuthGuard] },
    { path: 'privatepolicy', component: privatePolicy_component_1.privatePolicyComponent, canActivate: [index_4.AuthGuard] },
    { path: '**', redirectTo: '' }
];
exports.routing = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=app.routing.js.map