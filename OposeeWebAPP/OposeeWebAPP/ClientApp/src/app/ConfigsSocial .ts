import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { SocialLoginModule, AuthServiceConfig, GoogleLoginProvider, FacebookLoginProvider, } from "angular5-social-login";

@Injectable()
export class ConfigsSocial {


  constructor() {

  }

  getAuthServiceConfigs() {
    let config = new AuthServiceConfig(
      [
        {
          id: FacebookLoginProvider.PROVIDER_ID,
          provider: new FacebookLoginProvider("1037113233139121")
        },
        {
          id: GoogleLoginProvider.PROVIDER_ID,
          provider: new GoogleLoginProvider("Your-Google-Client-Id")
        },
      ]
    )
    return config;
  }




}



//{
//  "installed": {
//    "client_id": "443938519466-csfdkldsq00f7ljujdm67teuu378mron.apps.googleusercontent.com",

//      "project_id": "opozeetestapp",
//        "auth_uri": "https://accounts.google.com/o/oauth2/auth",
//          "token_uri": "https://oauth2.googleapis.com/token",
//            "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
//              "client_secret": "wz4hTv6_WujhCAK7U6_O7R0X",
//                "redirect_uris": ["urn:ietf:wg:oauth:2.0:oob", "http://localhost"]
//  }
//}

//{ "web": { "client_id": "443938519466-c1bekm3bjli41fp2pe166pm4klvjgtnr.apps.googleusercontent.com", "project_id": "opozeetestapp", "auth_uri": "https://accounts.google.com/o/oauth2/auth", "token_uri": "https://oauth2.googleapis.com/token", "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs", "client_secret": "-YvdHHceVxzTocefCxGD5tqr", "redirect_uris": ["http://localhost:4200/login"] } }


//26-march
//{ "web": { "client_id": "4673918337-dj94ciphlc0u3taumeqeht8b3pu2vdae.apps.googleusercontent.com", "project_id": "opozee-235606", "auth_uri": "https://accounts.google.com/o/oauth2/auth", "token_uri": "https://oauth2.googleapis.com/token", "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs", "client_secret": "oyAVtpn2VeSKsxE7BsPG_R-m", "redirect_uris": ["https://opozee-235606.firebaseapp.com/__/auth/handler"], "javascript_origins": ["http://localhost", "http://localhost:5000", "https://opozee-235606.firebaseapp.com", "http://23.111.138.246:8031"] } }
