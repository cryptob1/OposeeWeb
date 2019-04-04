import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../_services/user.service';
import { first } from 'rxjs/operators';


@Component({ templateUrl: 'userpostQuestion.component.html' })
export class UserpostQuestion implements OnInit {

  Id: number;
  userProfiledata: {}


  constructor(private route: ActivatedRoute, private userService: UserService) {
    if (this.route.snapshot.params["Id"]) {
      this.Id = this.route.snapshot.params["Id"];
    }
  }



  getUserProfile() {
    //var Id = this.localStorageUser.Id;
    this.userService.getUserProfileWeb(this.Id).pipe(first()).subscribe(data => {
      debugger;
      this.userProfiledata = data
      console.log(data);
    });
  }


  ngOnInit() {
  // this.getUserProfile();
  }






}
