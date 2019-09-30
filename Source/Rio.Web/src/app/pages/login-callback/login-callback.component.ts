import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'rio-login-callback',
  templateUrl: './login-callback.component.html',
  styleUrls: ['./login-callback.component.scss']
})
export class LoginCallbackComponent implements OnInit {
  private watchUserChangeSubscription: any;

  constructor(private router: Router, private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      var redirectUrl = sessionStorage.getItem("authRedirectUrl");
      console.log(redirectUrl);
      sessionStorage.removeItem("authRedirectUrl");
      this.router.navigate([redirectUrl]);
    });
  }
}
