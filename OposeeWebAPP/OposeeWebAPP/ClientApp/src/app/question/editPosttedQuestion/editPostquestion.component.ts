import { Component, OnInit } from '@angular/core';
import { User, LocalStorageUser } from '../../_models';
import { UserService, AlertService } from '../../_services';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { debounce } from 'rxjs/operator/debounce';
import { ToastrService } from 'ngx-toastr';

@Component({ templateUrl: 'editPostquestion.component.html', styleUrls: ['./editPostquestion.component.css'] })
export class EditPostquestion implements OnInit {
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
  //editquestion
  QuestionId: number = 0;
  

  constructor(private userService: UserService, private userservice: UserService, private alertService: AlertService, private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService,

  ) {
    this.localStorageUser = JSON.parse(localStorage.getItem('currentUser'));
    if (this.route.snapshot.params["qId"]) {
      this.QuestionId = this.route.snapshot.params["qId"];
    }
  }

  ngOnInit() {

    this.questionPostForm = this.formBuilder.group({
      postQuestion: ['', Validators.required],
      hashtags: [''],
      ownerUserId: this.localStorageUser.Id,
      Id: this.QuestionId
    });
    this.getpostedEditQuestionweb()

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
    this.questionPostForm.value.postQuestion = this.questionPostForm.value.postQuestion.trim();
    let postQuestionTemp = this.questionPostForm.value.postQuestion.trim();
       
    if (postQuestionTemp == '') {
      postQuestionTemp = 0;
      this.toastr.error('', 'please Fill all Field', { timeOut: 2000 });
      return
    }
           
    this.loading = true;
    this.userservice.editPostQuestionwebService(this.questionPostForm.value)
      .pipe(first())
      .subscribe(data => {
        debugger;
        if (data) {
          this.alertService.success('Question Posted', true);
          this.router.navigate(['/postedQuestionEditList', data.Id]);
        }
        else {
          this.toastr.error('Wrong Record', 'some error occured.', { timeOut: 5000 });
        }
        this.loading = false;
   ;

      },
        error => {
          this.alertService.error(error);
          this.loading = false;
        });
  }



  getpostedEditQuestionweb() {
    debugger;
     this.userService.getpostedQuestionwebService(this.QuestionId ).pipe(first()).subscribe(data => {
       debugger;


      this.questionPostForm.controls['postQuestion'].setValue(data.PostQuestion);
      this.questionPostForm.controls['hashtags'].setValue(data.HashTags);



    });
  }

}
