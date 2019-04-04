import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../_services/user.service';
import { first } from 'rxjs/operators';
import { PostQuestionDetail, BookMarkQuestion, LocalStorageUser} from '../_models/user';
import { debounce } from 'rxjs/operator/debounce';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'bookmark-component',
  templateUrl: './bookmark.component.html',
  styleUrls: ['./bookmark.component.css']
})

export class BookmarkQuestion implements OnInit {
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
  PostQuestionDetailList: PostQuestionDetail[] = [];
  localStorageUser: LocalStorageUser;
  isNotRecordLoaded: boolean;
  dataModel = {
    'QuestId': 0, 'Comment': '',
    'CommentedUserId': 0,
    'Likes': 0,
    'OpinionAgreeStatus': 0,
    'Dislikes': 0,
    'CommentId': 0,
    'CreationDate': new Date()
  }

  
  // PostQuestionDetailModel: { 'Comments': [], 'PostQuestionDetail':{}};
  PostQuestionDetailModel: BookMarkQuestion = new BookMarkQuestion();

  // isExpanded = false;
  constructor(private route: ActivatedRoute, private userService: UserService, private formBuilder: FormBuilder, private router: Router, private toastr: ToastrService ) {
    this.localStorageUser = JSON.parse(localStorage.getItem('currentUser'));

    if (this.route.snapshot.params["Id"]) {
      this.Id = this.route.snapshot.params["Id"];
    }



    this.localStorageUser = JSON.parse(localStorage.getItem('currentUser'));
  }

  //logout() { }
  //toggle() {
  //  this.isExpanded = !this.isExpanded;
  //}

  ngOnInit() {
    this.postOpinionForm = this.formBuilder.group({
      firstName: ['', Validators.required],

    });
   this.GetAllOpinionWeb()

  }


  GetAllOpinionWeb() {

    debugger;
    this.userService.getAllBookMarkService(this.localStorageUser.Id).subscribe(data => {
      debugger;
      this.PostQuestionDetailList = data;
      //console.log(this.PostQuestionDetailModel);
    });


  }

  getQuestionDetail() {

    debugger;
    this.userService.getquestionDetails(this.Id, this.localStorageUser.Id).subscribe(data => {
      debugger;


      //console.log(this.PostQuestionDetailModel);
    });

  }

  saveLikeclick(index) {
    debugger;
    if (this.dataModel.Likes != 1 || this.imageShowLike  == -2) {
      this.imageShowLike = index;
      this.dataModel.Likes = 1;
      this.imageShowDislike = -2;
      /////
      this.dataModel.QuestId = this.Id;
      this.dataModel.Comment = this.comment;
      this.dataModel.CommentedUserId = this.localStorageUser.Id;
      this.dataModel.OpinionAgreeStatus = 1
      this.Isclicked = true;
      this.dataModel.CommentId = this.PostQuestionDetailModel.comments[index].Id;
      this.dataModel.CreationDate = new Date();
      this.SaveLikeDislike(this.dataModel);

    }
    else if (this.imageShowLike == index) {
      this.imageShowLike = -1;
      this.dataModel.Likes = 0;
    }
  }

  

  saveDislikeclick(index) {
    debugger;
    if (this.dataModel.Dislikes != 1 || this.imageShowDislike == -2) {
      this.imageShowDislike = index;
      this.dataModel.Dislikes = 1;
      this.imageShowLike = -2;

      this.dataModel.QuestId = this.Id;
      this.dataModel.Comment = this.comment;
      this.dataModel.CommentedUserId = this.localStorageUser.Id;
      this.dataModel.OpinionAgreeStatus = 0;
      this.Isclicked = true;
      this.dataModel.CommentId = this.PostQuestionDetailModel.comments[index].Id;
      this.dataModel.CreationDate = new Date();


      this.SaveLikeDislike(this.dataModel);
    }
    else if (this.imageShowDislike == index) {
      this.imageShowDislike = -1;
      this.dataModel.Dislikes = 0;
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
        this.loading = false;
        this.router.navigate(['/questiondetail/', this.Id]);
      },
        error => {
          //this.alertService.error(error);
          //this.loading = false;
        });
  }



  SaveLikeDislike(dataModel) {
    debugger;
    this.userService.SaveLikeDislikeService(this.dataModel)
      .pipe(first())
      .subscribe(data => {
        debugger;
        this.loading = false;
        this.router.navigate(['/questiondetail/', this.Id]);
      },
        error => {
          //this.alertService.error(error);
          //this.loading = false;
        });

  }

  showSuccess() {

    this.toastr.success('Hello world!', '');
  }

}
