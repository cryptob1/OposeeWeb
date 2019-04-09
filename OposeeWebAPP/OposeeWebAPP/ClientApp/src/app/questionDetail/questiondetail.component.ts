import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../_services/user.service';
import { first } from 'rxjs/operators';
import { LocalStorageUser } from '../_models/user';
import { PostQuestionDetail, BookMarkQuestion } from '../_models/user';
import { debounce } from 'rxjs/operator/debounce';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'questiondetail-component',
  templateUrl: './questiondetail.component.html',
  styleUrls: ['./questiondetail.component.css']
})

export class Questiondetail implements OnInit {
  model: any = {};
  postOpinionForm: FormGroup;
  loading = false;
  returnUrl: string;
  isAuthenticate: boolean;
  Id: number;
  Isclicked: boolean = false;
  comment: '';
  submitted: boolean = false;
  imageShowLike: number = -1;;
  imageShowDislike: number = -1;
  isWanttoSentComment: boolean = false;

  dataModel = {
    'QuestId': 0, 'Comment': '',
    'CommentedUserId': 0,
    'Likes': 0,
    'OpinionAgreeStatus': 0,
    'Dislikes': 0,
    'CommentId': 0,
    'CreationDate': new Date(),
    'LikeOrDislke': false,
  }

  localStorageUser: LocalStorageUser;
  // PostQuestionDetailModel: { 'Comments': [], 'PostQuestionDetail':{}};
  PostQuestionDetailModel: BookMarkQuestion = new BookMarkQuestion();
  // isExpanded = false;
  constructor(private route: ActivatedRoute, private userService: UserService, private formBuilder: FormBuilder, private router: Router,
    private toastr: ToastrService
  ) {
    this.localStorageUser = JSON.parse(localStorage.getItem('currentUser'));

    if (this.route.snapshot.params["Id"]) {
      this.Id = this.route.snapshot.params["Id"];
    }
  }

  //logout() { }
  //toggle() {
  //  this.isExpanded = !this.isExpanded;
  //}

  ngOnInit() {
    this.postOpinionForm = this.formBuilder.group({
      firstName: ['', Validators.required],
    });

    this.getQuestionDetail()
  }
    
  getQuestionDetail() {
    //debugger;
    this.userService.getquestionDetails(this.Id, this.localStorageUser.Id).subscribe(data => {
      //debugger;
      this.PostQuestionDetailModel = data as BookMarkQuestion;
      this.PostQuestionDetailModel.comments = data['Comments'];
      this.PostQuestionDetailModel.postQuestionDetail = data['PostQuestionDetail'];
      //console.log('data',this.PostQuestionDetailModel);
    });
  }

  saveLikeclick(Likes, index) {
    debugger;
    if (!Likes) {
      this.imageShowLike = index;
      this.imageShowDislike = -2;
      /////+
      this.dataModel.Likes = 1;
      this.dataModel.Dislikes = 0;
      this.dataModel.QuestId = this.Id;
      this.dataModel.Comment = this.comment;
      this.dataModel.CommentedUserId = this.localStorageUser.Id;
      this.dataModel.LikeOrDislke = true;
      //this.Isclicked = true;
      this.dataModel.CommentId = this.PostQuestionDetailModel.comments[index].Id;
      this.dataModel.CreationDate = new Date();
      this.SaveLikeDislike(this.dataModel);

    }
    else {
      this.imageShowLike = -1;

      this.dataModel.Likes = 0;
      this.dataModel.Dislikes = 1;
      this.dataModel.QuestId = this.Id;
      this.dataModel.Comment = this.comment;
      this.dataModel.CommentedUserId = this.localStorageUser.Id;
      this.dataModel.LikeOrDislke = true;
      //this.Isclicked = true;
      this.dataModel.CommentId = this.PostQuestionDetailModel.comments[index].Id;
      this.dataModel.CreationDate = new Date();
      this.SaveLikeDislike(this.dataModel);


    }
  }



  saveDislikeclick(DisLikes, index) {
    debugger;
    if (!DisLikes) {
      this.imageShowDislike = index;
      this.imageShowLike = -2;

      this.dataModel.Dislikes = 1;

      this.dataModel.Likes = 0;
      this.dataModel.QuestId = this.Id;
      this.dataModel.Comment = this.comment;
      this.dataModel.CommentedUserId = this.localStorageUser.Id;
      this.dataModel.OpinionAgreeStatus = 0;
      this.dataModel.LikeOrDislke = false;
      //this.Isclicked = true;
      this.dataModel.CommentId = this.PostQuestionDetailModel.comments[index].Id;
      this.dataModel.CreationDate = new Date();
      this.SaveLikeDislike(this.dataModel);
    }
    else {
      this.imageShowDislike = -1;

      this.dataModel.Dislikes = 0;
      this.dataModel.Likes = 1;
      this.dataModel.QuestId = this.Id;
      this.dataModel.Comment = this.comment;
      this.dataModel.CommentedUserId = this.localStorageUser.Id;
      this.dataModel.OpinionAgreeStatus = 0;
      this.dataModel.LikeOrDislke = false;
      //  this.Isclicked = true;
      this.dataModel.CommentId = this.PostQuestionDetailModel.comments[index].Id;
      this.dataModel.CreationDate = new Date();
      this.SaveLikeDislike(this.dataModel);


    }

  }

  clickYes() {
    this.dataModel.QuestId = this.Id;
    this.dataModel.Comment = this.comment;
    this.dataModel.CommentedUserId = this.localStorageUser.Id
    this.dataModel.OpinionAgreeStatus = 1
    this.Isclicked = true;

  }

  clickNO() {
    this.dataModel.QuestId = this.Id;
    this.dataModel.Comment = this.comment;
    this.dataModel.CommentedUserId = this.localStorageUser.Id
    this.dataModel.OpinionAgreeStatus = 0
    this.Isclicked = true;


  }

  cancelclick() {
    this.Isclicked = false;
    this.isWanttoSentComment = false;
  }

  saveOpinionclick() {

    debugger;
    this.submitted = true;
    debugger;
    if (this.dataModel.Comment == '' || this.dataModel.Comment == undefined) {
      return;
    }


    this.loading = true;
    this.userService.saveOpinionPost(this.dataModel)
      .pipe(first())
      .subscribe(data => {
        debugger;
        if (data.BalanceToken <= 0) {

          this.toastr.error('Token Blance 0', 'You have 0 tokens in your account. Please email us to refill the account to post opinion.', { timeOut: 5000 });
        }
        else {
          this.getQuestionDetail();
          this.toastr.success('Data save successfully', '');
        }
       
        this.loading = false;
        this.Isclicked = false;


      },
        error => {
          //this.alertService.error(error);
          //this.loading = false;
        });
  }



  SaveLikeDislike(dataModel) {
    debugger;
    this.loading = true;
    this.userService.SaveLikeDislikeService(this.dataModel)
      .pipe(first())
      .subscribe(data => {
        debugger;
        this.loading = false;
        this.getQuestionDetail();
        this.toastr.success('Data save successfully', '');
        this.dataModel = {
          'QuestId': 0, 'Comment': '',
          'CommentedUserId': 0,
          'Likes': 0,
          'OpinionAgreeStatus': 0,
          'Dislikes': 0,
          'CommentId': 0,
          'CreationDate': new Date(),
          'LikeOrDislke': false,
        }
        //this.router.navigate(['/questiondetail/', this.Id]);

      },
        error => {
          //this.alertService.error(error);
          //this.loading = false;
        });

  }


  saveBookmarkQuestion(IsBookmark) {

    debugger;
    var dataBookMarkModel = {
      'QuestionId': this.Id,
      'IsBookmark': true,
      'UserId': this.localStorageUser.Id,
      'CreationDate': new Date(),
      'ModifiedDate': new Date(),
    }
    if (IsBookmark) {
      dataBookMarkModel.IsBookmark = false;
    }
    else {

      dataBookMarkModel.IsBookmark = true;
    }


    this.getQuestionDetail()
    this.loading = true;
    this.userService.saveBookmarkQuestionservice(dataBookMarkModel)
      .pipe(first())
      .subscribe(data => {
        debugger;
        if (data.BalanceToken <= 0) {
          this.toastr.error('Token Blance 0', 'You have 0 tokens in your account. Please email us to refill the account to post opinion.', { timeOut: 5000 });
        }
        else {
          this.getQuestionDetail();
          this.toastr.success('Data save successfully', '');
        }
 


      },
        error => {
          this.loading = false;
        });


  }








  showSuccess() {

    this.toastr.success('Hello world!', '');
  }

  commentSend() {
    this.isWanttoSentComment = true

  }
}
