import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';

import { AlertService, AuthenticationService, UserService } from '../_services';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DataSharingService } from '../dataSharingService';
import { ToastrService } from 'ngx-toastr';
import { AuthService, FacebookLoginProvider, GoogleLoginProvider } from 'angular5-social-login';
//import { AuthService } from './auth/auth.service'; 
import * as AuthService1 from "auth0-js";

@Component({ templateUrl: 'login.component.html' })
export class LoginComponent implements OnInit {


  Counter: number = 0;

  loginForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;
  thirdPartyModel = { 'UserName': '', 'FirstName': '', 'LastName': '', 'DeviceType': 'Web', 'ThirdPartyId': '', 'Email': '', 'ThirdPartyType': 0 };

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private dataSharingService: DataSharingService,
    private toastr: ToastrService,
    private socialAuthService: AuthService,
    public auth: AuthService
  ) {


  }

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required]
    });

    // reset login status
    this.authenticationService.logout();

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  // convenience getter for easy access to form fields
  get f() { return this.loginForm.controls; }

  onSubmit() {
    this.submitted = true;
    debugger;
    // stop here if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    this.authenticationService.login(this.loginForm.value)
      .pipe(first())
      .subscribe(data => {
        debugger;
        if (data.Id != 0) {

          this.loading = false;
          this.toastr.success('Login', 'login successfully!', { timeOut: 1000 });
          this.dataSharingService.loginsetstate(data);
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          localStorage.setItem('currentUser', JSON.stringify(data));
          this.router.navigate(['/questionlisting']);
        }

        else {
          this.loading = false;
          this.toastr.error('Invalid User', 'please check user name or Password !', { timeOut: 2000 });
        }
      },
        error => {
          this.alertService.error(error);
          this.loading = false;
        });
  }


  socialSignIn(socialPlatform: string) {
    debugger;
    let socialPlatformProvider;
    if (socialPlatform == "facebook") {
      socialPlatformProvider = FacebookLoginProvider.PROVIDER_ID;

      this.socialAuthService.signIn(socialPlatformProvider).then(
        (userData) => {
          debugger;
          this.thirdPartyModel.ThirdPartyId = userData.id;
          this.thirdPartyModel.FirstName = userData.name.split(' ')[0];
          this.thirdPartyModel.LastName = userData.name.split(' ')[1];
          this.thirdPartyModel.Email = userData.email;
          this.thirdPartyModel.ThirdPartyType = 0;

          this.authenticationService.loginWithFacebook(this.thirdPartyModel).pipe(first()).subscribe(data => {
            debugger;
            if (data) {

              this.loading = false;
              this.toastr.success('Login', 'login successfully!', { timeOut: 1000 });
              this.dataSharingService.loginsetstate(data);
              // store user details and jwt token in local storage to keep user logged in between page refreshes
              localStorage.setItem('currentUser', JSON.stringify(data));
              this.router.navigate(['/questionlisting']);
            }

            else {
              this.loading = false;
              this.toastr.error('Invalid User', 'please check user name or Password !', { timeOut: 2000 });
            }
          },
            error => {
              this.alertService.error(error);
              this.loading = false;
            })


          // Now sign-in with userData

        }
      );

    } else if (socialPlatform == "google") {


      socialPlatformProvider = GoogleLoginProvider.PROVIDER_ID;

      this.socialAuthService.signIn(socialPlatformProvider).then(
        (userData) => {

          this.thirdPartyModel.ThirdPartyId = userData.id;
          this.thirdPartyModel.FirstName = userData.name.split(' ')[0];
          this.thirdPartyModel.LastName = userData.name.split(' ')[1];
          this.thirdPartyModel.Email = userData.email;
          this.thirdPartyModel.ThirdPartyType = 2;
          debugger;
          this.authenticationService.loginWithGoogle(this.thirdPartyModel).pipe(first()).subscribe(data => {
            debugger;
            if (data) {
              console.log(data)

              this.loading = false;
              this.toastr.success('Login', 'login successfully!', { timeOut: 1000 });
              this.dataSharingService.loginsetstate(data);
              // store user details and jwt token in local storage to keep user logged in between page refreshes
              localStorage.setItem('currentUser', JSON.stringify(data));
              this.router.navigate(['/questionlisting']);
            }

            else {
              this.loading = false;
              this.toastr.error('Invalid User', 'please check user name or Password !', { timeOut: 2000 });
            }
          },
            error => {
              debugger;
              alert()
              this.alertService.error(error);
              this.loading = false;
            })

          // Now sign-in with userData

        }, error => {
          debugger;
          alert()
          this.alertService.error(error);
          this.loading = false;
        }
      )
    }

  }

  loginwithTwitteSecond(event) {
    debugger
    var data = event.currentTarget.value.split('_')



    this.thirdPartyModel.ThirdPartyId = data[0];
    this.thirdPartyModel.Email = data[1];
    this.thirdPartyModel.FirstName = data[2].split(' ')[0];
    this.thirdPartyModel.LastName = data[2].split(' ')[1];
    this.thirdPartyModel.ThirdPartyType = 1;
    this.authenticationService.loginWithGoogle(this.thirdPartyModel).pipe(first()).subscribe(data => {
      debugger;
      if (data) {
        this.loading = false;
        this.toastr.success('Login', 'login successfully!', { timeOut: 1000 });
        this.dataSharingService.loginsetstate(data);
        // store user details and jwt token in local storage to keep user logged in between page refreshes
        localStorage.setItem('currentUser', JSON.stringify(data));
        this.router.navigate(['/questionlisting']);
      }
      else {
        this.loading = false;
        this.toastr.error('Invalid User', 'please check user name or Password !', { timeOut: 2000 });
      }
    },
      error => {
        this.alertService.error(error);
        this.loading = false;
      })


  }
}



