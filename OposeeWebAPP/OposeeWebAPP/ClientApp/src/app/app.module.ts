import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import { AlertComponent } from './_directives/index';
import { AuthGuard } from './_guards/index';
import { JwtInterceptor } from './_helpers/index';
import { AlertService, AuthenticationService, UserService } from './_services/index';
import { HomeComponent } from './home/index';
import { LoginComponent } from './login/index';
import { RegisterComponent } from './register/index';
import { routing } from './app.routing'
import { HttpModule } from '@angular/http';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { QuestionListingComponent } from './question/QuestionListing';
import { PostQuestionComponent } from './question/PostQuestion';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { NotificationComponent } from './notification/notification.component';
import { AppConfigService } from './appConfigService';
import { HeaderComponent } from './header/header.component';
import { Questiondetail } from './questionDetail/questiondetail.component';
import { ProfileComponent } from './user/profile/profile.component';
import { EditProfileComponent } from './user/editprofile/editProfile.component';
import { DataSharingService } from './dataSharingService';
import { ConfigsSocial } from './ConfigsSocial ';
import { UserpostQuestion } from './user/userpostedQuestion/userpostQuestion.component';
import { PostedQuestionEditList } from './question/PostedQuestion/postedQuestionEditList.component';
import { EditPostquestion } from './question/editPosttedQuestion/editPostquestion.component';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import {
  SocialLoginModule,
  AuthServiceConfig,
  GoogleLoginProvider,
  FacebookLoginProvider,
 
} from "angular5-social-login";
import { BookmarkQuestion } from './bookmark/bookmark.component';
import { termandConditionComponent } from './legal/termandCondition.component';

import { privatePolicyComponent } from './legal/privatePolicy.component';


export function getAuthServiceConfigs() {
  let config = new AuthServiceConfig(
    [
      {
        //600720573732817
        id: FacebookLoginProvider.PROVIDER_ID,
        provider: new FacebookLoginProvider("600720573732817")
      },
      {
        id: GoogleLoginProvider.PROVIDER_ID,
        provider: new GoogleLoginProvider("803302437724-sg4e9amg0crda1m4m8g1g650ddsa0hoo.apps.googleusercontent.com")
      },
    ]
  )
  return config;
}

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    HttpModule,
    routing,
    NgMultiSelectDropDownModule.forRoot(),
    BrowserAnimationsModule, // required animations module
    ToastrModule.forRoot(),
    SocialLoginModule,

  
  ],
  declarations: [
    AppComponent,
    AlertComponent,
    HomeComponent,
    LoginComponent,
    RegisterComponent,
    QuestionListingComponent,
    PostQuestionComponent,
    NavMenuComponent,
    NotificationComponent,
    HeaderComponent,
    Questiondetail,
    ProfileComponent,
    EditProfileComponent,
    UserpostQuestion, BookmarkQuestion, PostedQuestionEditList, EditPostquestion, termandConditionComponent
    , privatePolicyComponent
  ],
  providers: [
    AuthGuard,
    AlertService,
    AuthenticationService,
    AppConfigService,
    UserService,
    DataSharingService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
    {
      provide: AuthServiceConfig,
      useFactory: getAuthServiceConfigs
    }
    // provider used to create fake backend
   
  ],

  bootstrap: [AppComponent]
})

export class AppModule { }

