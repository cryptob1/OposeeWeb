import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DataSharingService } from '../dataSharingService';
import { User, LocalStorageUser, PostQuestionDetail } from '../_models/user';

  @Component({
    selector: 'privatePolicy-Component',
    templateUrl: './privatePolicy.Component.html',
    styleUrls: ['./privatePolicy.Component.css']
  })

export class privatePolicyComponent implements OnInit {
  model: any = {};
  loading = false;
  returnUrl: string;
    isAuthenticate: boolean;
    isAuthenticateUserId: number = 0;
    ImageURL: string = '';
    localStorageUser: LocalStorageUser;
    loginData: any;
 // isExpanded = false;
    constructor(private dataSharingService: DataSharingService ,private route: ActivatedRoute,private router: Router,) {

    this.localStorageUser = JSON.parse(localStorage.getItem('currentUser'));

 }

  logout() {

    localStorage.removeItem('currentUser');
    window.location.reload();

  }

    ngOnInit() {

      debugger;
      this.dataSharingService.loginget.subscribe(data => {
            this.loginData = data
      })
   
      if (this.localStorageUser && this.localStorageUser.Id > 0) {
        this.isAuthenticateUserId = this.localStorageUser.Id;
        this.loginData = this.localStorageUser
     
 
      }
  }
}
