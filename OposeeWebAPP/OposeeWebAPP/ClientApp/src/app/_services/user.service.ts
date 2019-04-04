import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RequestOptions } from '@angular/http';
import { Http, Headers, Response } from '@angular/http';
import { HttpHeaders } from '@angular/common/http';
import { User, Question, NotificationsModel, PostQuestionDetail, BookMarkQuestion, UserProfileModel } from '../_models';
import { UserEditProfileModel } from '../_models/user';
import { QuestionListing } from '../_models/question';
import { config } from 'process';
import { AppConfigService } from '../appConfigService';
import { Notification, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class UserService {

  myAppUrl: string = ""
  constructor(private http: HttpClient, appConfigService: AppConfigService) {
    //let headers: Headers = new Headers();

    //headers.append('Access-Control-Allow-Origin', '*');
    //headers.append('Access-Control-Allow-Methods', 'GET,PUT,POST,DELETE');
    //headers.append('Authorization', 'Basic b3Bvc2VlOm9wb3NlZTk5IQ==');
    //let options = new RequestOptions({ headers: headers });


    this.myAppUrl = appConfigService.baseURL;
  }

  //const httpOptions = {
  //  headers: new HttpHeaders({
  //    'Content-Type': 'application/json',
  //    'Authorization': 'my-auth-token'
  //  })
  //}
  getAllquestion() {
    return this.http.get<QuestionListing[]>(this.myAppUrl + 'oposee/api/WebApi/GetQuestion', {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })
  }

  getById(id: number) {
    return this.http.get(this.myAppUrl + 'users/ + id');
  }

  register(user: User) {
    return this.http.post(this.myAppUrl + 'oposee/api/WebApi/RegisterUser', user, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })
  }

  update(user: User) {
    return this.http.put(this.myAppUrl + 'users/' + user.id, user);
  }

  delete(id: number) {
    return; //this.http.delete(`${config.apiUrl}/users/` + id);
  }


  getUserRecords() {
    return this.http.get<QuestionListing[]>(this.myAppUrl + 'oposee/api/WebApi/GetUserALLRecords', {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })
  }

  postQuestionweb(question) {
    return this.http.post<any>(this.myAppUrl + 'oposee/api/WebApi/postquestionweb', question, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })

  }






  getAllNotification(Model) {
    return this.http.post<NotificationsModel[]>(this.myAppUrl + 'oposee/api/WebApi/GetAllNotificationByUser',Model, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })
  }


  //getAllQuestionlist(questionGetModel) {
  //  return this.http.get<any>(this.myAppUrl + 'oposee/api/WebApi/GetAllPostsWeb?UserId=' + userId + '&Search=' + search +'&pageNumber'+, {
  //    headers: new HttpHeaders({
  //      'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='

  //    })

  //  })
  //}

  getAllQuestionlist(questionGetModel) {
    return this.http.post<any>(this.myAppUrl + 'oposee/api/WebApi/GetAllPostsWeb', questionGetModel, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })

  }









  getquestionDetails(Id: number, UserId: number) {
    return this.http.get<any>(this.myAppUrl + 'oposee/api/WebApi/GetAllOpinionWeb?questId=' + Id + '&UserId=' + UserId, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    }).pipe(map(data => { return data }
    ))
  }


  getUserProfileWeb(userid: number) {
    return this.http.get<UserProfileModel>(this.myAppUrl + 'oposee/api/WebApi/GetUserProfileWeb?userid=' + userid, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    }).pipe(map(profiledata => { return profiledata }

    ))
  }

  editUserprofile(Model) {
    return this.http.post(this.myAppUrl + 'oposee/api/WebApi/EditUserProfileWeb', Model, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })

  }

  uploadfileService(userId, fileToUpload: File) {
    debugger;
    const formData: FormData = new FormData();
    formData.append('Image', fileToUpload, fileToUpload.name);
    formData.append('userId', userId);
    return this.http.post(this.myAppUrl + 'oposee/api/WebApi/UploadProfileWeb', formData, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })

  }


  //UserEditProfileModel
  getEditUserProfileWeb(userid: number) {
    return this.http.get<any>(this.myAppUrl + 'oposee/api/WebApi/GetEditUserProfileWeb?userid=' + userid, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    }).pipe(map(profiledata => { return profiledata }

    ))
  }




  saveOpinionPost(Model) {
    return this.http.post<any>(this.myAppUrl + 'oposee/api/WebApi/PostOpinionWeb', Model, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })

  }


  //UserEditProfileModel
  GetAllTaggedDropService() {
    return this.http.get<any>(this.myAppUrl + 'oposee/api/WebApi/GetAllTaggedDropWeb', {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    }).pipe(map(profiledata => { return profiledata }

    ))
  }

  //savelike or dislkie
  SaveLikeDislikeService(Model) {
    return this.http.post(this.myAppUrl + 'oposee/api/WebApi/PostLikeDislikeWeb', Model, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })

  }

  saveBookmarkQuestionservice(Model) {

    return this.http.post<any>(this.myAppUrl + 'oposee/api/WebApi/BookMarkQuestionWeb', Model, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })

    })
  }


  getAllBookMarkService(userid: number) {

    return this.http.get<any>(this.myAppUrl + 'oposee/api/WebApi/GetAllBookMarkWebById/?userid=' + userid, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })

    })
  }

  getquestionDetails1(Id: number, UserId: number) {
    return this.http.get<any>(this.myAppUrl + 'oposee/api/WebApi/GetAllOpinionWeb?questId=' + Id + '&UserId=' + UserId, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    }).pipe(map(data => { return data }
    ))
  }

  getQuestionListEditService(Model) {
    return this.http.post<any>(this.myAppUrl + 'oposee/api/WebApi/GetAllPostsQuestionEditWeb' , Model , {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    }).pipe(map(data => { return data }
    ))
  }
  

  getpostedQuestionwebService(QuestionId) {
    return this.http.get<any>(this.myAppUrl + 'oposee/api/WebApi/GetPostedQuestionEditWeb?QuestionId=' + QuestionId, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })

  }

  editPostQuestionwebService(question) {
    return this.http.post<any>(this.myAppUrl + 'oposee/api/WebApi/EditPostQuestionWeb', question, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })

  }

  deletePostQuestionwebService(question) {
    return this.http.post<any>(this.myAppUrl + 'oposee/api/WebApi/DeletePostQuestionWeb', question, {
      headers: new HttpHeaders({
        'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
      })
    })

  }

}


