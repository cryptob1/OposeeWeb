import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './home/index';
import { LoginComponent } from './login/index';
import { RegisterComponent } from './register/index';
import { AuthGuard } from './_guards/index';
import { PostQuestionComponent } from './question/PostQuestion';
import { QuestionListingComponent } from './question/QuestionListing';
import { NotificationComponent } from './notification/notification.component';
import { Questiondetail } from './questionDetail/questiondetail.component';
import { ProfileComponent } from './user/profile/profile.component';
import { EditProfileComponent } from './user/editprofile/editProfile.component';
import { UserpostQuestion } from './user/userpostedQuestion/userpostQuestion.component';
import { BookmarkQuestion } from './bookmark/bookmark.component';
import { PostedQuestionEditList } from './question/PostedQuestion/postedQuestionEditList.component';
import { EditPostquestion } from './question/editPosttedQuestion/editPostquestion.component';
import { termandConditionComponent } from './legal/termandCondition.component';
import { privatePolicyComponent } from './legal/privatePolicy.component';
const appRoutes: Routes = [
  { path: 'questionlisting/:search', component: QuestionListingComponent },
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'postquestion/:questionId', component: PostQuestionComponent, canActivate: [AuthGuard] },
  { path: 'notifications', component: NotificationComponent, canActivate: [AuthGuard] },
  // otherwise redirect to home
  { path: 'questiondetail/:Id', component: Questiondetail, canActivate: [AuthGuard] },
  { path: 'profile/:Id', component: ProfileComponent, canActivate: [AuthGuard] },
  { path: 'editprofile/:Id', component: EditProfileComponent, canActivate: [AuthGuard] },
  { path: 'mypostquestions', component: UserpostQuestion, canActivate: [AuthGuard] },

  { path: 'bookmark/:questionId', component: BookmarkQuestion, canActivate: [AuthGuard] },
  { path: 'postedQuestionEditList/:questionId', component: PostedQuestionEditList, canActivate: [AuthGuard] },
  { path: 'editpostedquestion/:qId', component: EditPostquestion, canActivate: [AuthGuard] },
  { path: 'termcondition', component: termandConditionComponent, canActivate: [AuthGuard] },
  { path: 'privatepolicy', component: privatePolicyComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '' }
];

export const routing = RouterModule.forRoot(appRoutes);
