import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

@Injectable()
export class DataSharingService {

  Login: number = 0
  private loginsource = new BehaviorSubject(this.Login);
  loginget = this.loginsource.asObservable();

  constructor() {

  }
  //logingetstate(option, value) {
  //  this.loginsource.next(message)
  //}

   loginsetstate(Login: any) :void {
     this.loginsource.next(Login);
  }



}
