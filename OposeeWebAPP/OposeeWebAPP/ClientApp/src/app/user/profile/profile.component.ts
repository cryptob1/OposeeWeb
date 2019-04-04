import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../_services/user.service';
import { first } from 'rxjs/operators';
import {  LocalStorageUser } from '../../_models';
import { UserProfileModel } from '../../_models/user';


@Component({ templateUrl: 'profile.component.html' })
export class ProfileComponent implements OnInit {

  Id: number;
  userProfiledata: UserProfileModel
  //userProfiledata: {}
  localStorageUser: LocalStorageUser

  constructor(private route: ActivatedRoute, private userService: UserService ) {
    if (this.route.snapshot.params["Id"]) {
      this.Id = this.route.snapshot.params["Id"];

      this.localStorageUser = JSON.parse(localStorage.getItem('currentUser'));
    }
  }



  getUserProfile() {
    //var Id = this.localStorageUser.Id;
    this.userService.getUserProfileWeb(this.localStorageUser.Id).pipe(first()).subscribe(data => {
      debugger;
      this.userProfiledata = data
      console.log(data);
    });
  }


  ngOnInit() {
   this.getUserProfile();
  }


  



}
