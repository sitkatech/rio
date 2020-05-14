import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'rio-login-callback',
  templateUrl: './login-callback.component.html',
  styleUrls: ['./login-callback.component.scss']
})
export class LoginCallbackComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;

  constructor(private router: Router, private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
        if (this.authenticationService.isUserAnAdministrator(currentUser)){
          window.localStorage.removeItem("activeAccount");
          this.authenticationService.setActiveAccount(undefined);
          this.router.navigate(['/manager-dashboard']);
        } else if (this.authenticationService.isUserALandOwnerOrDemoUser(currentUser)){
          this.router.navigate(['/landowner-dashboard']);
        } else{
          this.router.navigate(['/']);
        }
    });
  }

  ngOnDestroy(): void {
    this.watchUserChangeSubscription.unsubscribe();
  }
}
