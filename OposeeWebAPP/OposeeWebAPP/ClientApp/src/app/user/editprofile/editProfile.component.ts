import { Component, OnInit } from '@angular/core';
import { first } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../_services/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserEditProfileModel } from '../../_models/user';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http'
import { LocalStorageUser } from '../../_models';



@Component({ templateUrl: 'editProfile.component.html' })
export class EditProfileComponent implements OnInit {
  edtiProfileForm: FormGroup;
  loading: boolean = false;
  submitted = false;
  userId: number = 0;
  userEditProfileModel: UserEditProfileModel;
  public progress: number;
  public message: string;
  imageUrl: string = "";
  fileToUpload: File = null;
  localStorageUser: LocalStorageUser

  constructor(private userService: UserService, private formBuilder: FormBuilder, private route: ActivatedRoute) {
    if (this.route.snapshot.params["Id"]) {
      this.userId = this.route.snapshot.params["Id"];

      this.localStorageUser = JSON.parse(localStorage.getItem('currentUser'));
    }

  }

  ngOnInit() {
    this.edtiProfileForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', Validators.required],
      password: ['', Validators.required],
      userId: ['']
    });

    this.getUserProfile();

  }

  // convenience getter for easy access to form fields
  get f() { return this.edtiProfileForm.controls; }

  getUserProfile() {
    debugger;
    //var Id = this.localStorageUser.Id;
    this.userService.getEditUserProfileWeb(this.localStorageUser.Id).pipe(first()).subscribe(data => {
      debugger;
      //this.userEditProfileModel = data
      //this.edtiProfileForm.setValue(data);
      this.edtiProfileForm.controls['firstName'].setValue(data.FirstName);
      this.edtiProfileForm.controls['lastName'].setValue(data.LastName);
      this.edtiProfileForm.controls['email'].setValue(data.Email);
      this.edtiProfileForm.controls['password'].setValue(data.Password);
      this.edtiProfileForm.controls['userId'].setValue(data.UserId);
      this.imageUrl = data.ImageURL;
      
    });
  }

  onSubmit() {
    debugger;
    this.submitted = true;
    debugger;
    if (this.edtiProfileForm.invalid) {
      return;
    }


    this.loading = true;
    this.userService.editUserprofile(this.edtiProfileForm.value)
      .pipe(first())
      .subscribe(data => {
        debugger;
        this.loading = false;
      },
        error => {
          //this.alertService.error(error);
          //this.loading = false;
        });
  }


  handleFileInput(file: FileList) {
    this.fileToUpload = file.item(0);

    //Show image preview
    var reader = new FileReader();
    reader.onload = (event: any) => {
      this.imageUrl = event.target.result;
    }
    reader.readAsDataURL(this.fileToUpload);
  }



  UploadFile() {
    debugger;
    this.userService.uploadfileService(this.userId , this.fileToUpload).subscribe(
      data => {
        
        //Caption.value = null;
        //Image.value = null;
        this.getUserProfile();
        this.imageUrl = "../../../assets/images/user.png";
      }
    );
  }


 
}
