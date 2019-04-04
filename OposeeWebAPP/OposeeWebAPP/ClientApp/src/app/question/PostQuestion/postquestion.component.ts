import { Component, OnInit } from '@angular/core';
import { User, LocalStorageUser } from '../../_models';
import { UserService, AlertService } from '../../_services';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { debounce } from 'rxjs/operator/debounce';
import { ToastrService } from 'ngx-toastr';

@Component({ templateUrl: 'postquestion.component.html', styleUrls: ['./postquestion.component.css'] })
export class PostQuestionComponent implements OnInit {
  questionPostForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;

  localStorageUser: LocalStorageUser;

  dropdownList = [];
  selectedItems = [];
  dropdownSettings = {};
  Usertags: string = '';

  users: User[] = [];
  constructor(private userService: UserService, private userservice: UserService, private alertService: AlertService, private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService,

  ) {
    this.localStorageUser = JSON.parse(localStorage.getItem('currentUser'));
  }

  ngOnInit() {

    debugger;
    //this.loadAllUsers();
    this.questionPostForm = this.formBuilder.group({
      postQuestion: ['', Validators.required],
      hashtags: [''],
      ownerUserId: this.localStorageUser.Id,
      taggedUser: ['']
    });


    this.GetAllTaggedDrop(); // Tag dropdown bind

    //test

    //this.selectedItems = [];
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'UserID',
      textField: 'UserName',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 3,
      allowSearchFilter: true
    }

  }

  // convenience getter for easy access to form fields
  get f() { return this.questionPostForm.controls; }

  //PostQuestionWeb


  onSubmit() {
    this.submitted = true;
    debugger;
    // stop here if form is invalid
    if (this.questionPostForm.invalid) {
      this.toastr.error('', 'please Fill all Field !', { timeOut: 2000 });
      return;
    }
    let postQuestionTemp = this.questionPostForm.value.postQuestion.trim();
    let UsertagsTemp = this.Usertags.trim();
    this.questionPostForm.value.hashtags = this.questionPostForm.value.hashtags.trim();
    let taggedUserTemp = this.Usertags
       
    if (UsertagsTemp! == '' ||  postQuestionTemp == '') {
      postQuestionTemp = 0;
      this.toastr.error('', 'please Fill all Field', { timeOut: 2000 });
      return
    }


    this.Usertags = this.Usertags.substring(0, this.Usertags.length - 1)
    this.questionPostForm.value.taggedUser = this.Usertags;
    this.loading = true;
    this.userservice.postQuestionweb(this.questionPostForm.value)
      .pipe(first())
      .subscribe(data => {

        if (data.BalanceToken <= 0) {
          this.toastr.error('Token Blance 0', 'You have 0 tokens in your account. Please email us to refill the account to post opinion.', { timeOut: 5000 });
        }
        else {
          this.alertService.success('Question Posted', true);
          this.router.navigate(['/questionlisting', 0]);
        }
        this.loading = false;
        debugger;

      },
        error => {
          this.alertService.error(error);
          this.loading = false;
        });
  }
  onItemSelect(e) {
    debugger;
    this.Usertags += e.UserID + ','

  }

  onDeSelect(e) {
    debugger;
    var replacevalue = e.UserID + ','
    this.Usertags = this.Usertags.replace(replacevalue, '');

  }

  onSelectAll(e) {

    for (let i = 0; i < e.length; i++) {
      this.Usertags += e[i].UserID + ','
    }
    debugger
   

  }


  GetAllTaggedDrop() {
    debugger;
    //var Id = this.localStorageUser.Id;
    this.userService.GetAllTaggedDropService().pipe(first()).subscribe(data => {
      debugger;
      this.dropdownList = data;

      console.log(data);
    });
  }

}
