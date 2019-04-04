export class User {
  id: number;
  email: string;
  password: string;
  firstName: string;
  lastName: string;

}


export class Question {

  addQuestion: string;
  addHasTag: string;

}


export class LocalStorageUser {



  Id: number;
  email: string;
  UserName: string;
  ImageURL: string;


  constructor() {
    this.Id = 0;
    this.email = '',
      this.UserName = ''

  }
}


export class NotificationsModel {

  Id: Number;
  Username: string;
  Image: string;
  Like: boolean;
  Message: string;
  ModifiedDate: Date;
  Opinion: Date;
  OpinionId: number;
  TotalRecordcount: number;

}



export class BookMarkQuestion {
  public comments: Comments[] = [];
  public postQuestionDetail: PostQuestionDetail[] = [new PostQuestionDetail()];
  public IsBookmark: boolean;
  OwnerUserName: string

}

export class Comments {

  Comment: string;
  CommentedUserId: number;
  CommentedUserName: string;
  CreationDate: Date;
  DisLikes: boolean;
  DislikesCount: number;
  Id: number;
  IsAgree: boolean;
  Likes: boolean;
  LikesCount: number;
  Name: string;
  UserImage: string;

}

export class PostQuestionDetail {
  BookmarkId: number;
  CreationDate: Date;
  HashTags: string;
  Id: number;
  IsBookmark: boolean;
  IsUserPosted: boolean;
  OwnerUserID: number;
  OwnerUserName: string;
  Question: string;
  TotalDisLikes: number;
  TotalLikes: number;
  UserImage: string;

  constructor() {
    //this.IsBookmark = false;
  }



}

export class UserProfileModel {

  BalanceToken: number;
  Email: string;
  FirstName: string;
  ImageURL: string;
  LastName: string;
  Password: string;
  TotalDislikes: string;
  TotalLikes: number;
  TotalPostedQuestion: number;
  UserID: number;
  UserName: string;

}


export class UserEditProfileModel {

  Email: string;
  FirstName: string;
  ImageURL: string;
  LastName: string;
  Password: string;
  UserID: number;


}
