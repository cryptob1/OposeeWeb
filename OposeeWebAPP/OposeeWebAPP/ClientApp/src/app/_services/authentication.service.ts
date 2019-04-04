import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { map } from 'rxjs/operators';
import { config } from 'process';
import { AppConfigService } from '../appConfigService'

@Injectable()
export class AuthenticationService {
  myAppUrl: string = "";


  constructor(private http: HttpClient, appConfigService: AppConfigService, private router: Router, ) {
    this.myAppUrl = appConfigService.baseURL;
  }


  login(login) {
    debugger;
    //return this.http.post(`${config.apiUrl}/oposee/api/MobileApi/Login`, login, {
    //  headers: new HttpHeaders({
    //    'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ=='
    //  })
    //})
    var reqHeader = new HttpHeaders({ 'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ==' });
    return this.http.post<any>(this.myAppUrl + '/oposee/api/WebApi/Login', login, { headers: reqHeader })
      .pipe(map(user => {
        debugger;
        return user;
      }));
  }

  logout() {
    // remove user from local storage to log user out
    localStorage.removeItem('currentUser');
    //  this.router.navigate(['']);
  }

  loginWithFacebook(model) {

    var reqHeader = new HttpHeaders({ 'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ==' });
    return this.http.post<any>(this.myAppUrl + 'oposee/api/WebApi/SigninThirdPartyWeb', model, { headers: reqHeader })
      .pipe(map(user => {
        debugger;
        return user;
      }));

  }

  loginWithGoogle(model) {

    var reqHeader = new HttpHeaders({ 'Authorization': 'Basic b3Bvc2VlOm9wb3NlZTk5IQ==' });
    return this.http.post<any>(this.myAppUrl + 'oposee/api/WebApi/SigninThirdPartyWeb', model, { headers: reqHeader })
      .pipe(map(user => {
        debugger;
        return user;
      }));

  }


}
