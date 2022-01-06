import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'rio-login-callback',
  templateUrl: './login-callback.component.html',
  styleUrls: ['./login-callback.component.scss']
})
export class LoginCallbackComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;

  constructor(private router: Router, 
    private authenticationService: AuthenticationService,
    private userService: UserService) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      let authRedirectUrl = this.authenticationService.getAuthRedirectUrl();  
      if (authRedirectUrl && authRedirectUrl !== "/") {
          this.router.navigateByUrl(authRedirectUrl)
              .then(() => {
                  this.authenticationService.clearAuthRedirectUrl();
              });
        } 
        else if (this.authenticationService.isUserAnAdministrator(currentUser)){
          this.router.navigate(['/manager-dashboard']);
        } else if (this.authenticationService.isUserALandOwnerOrDemoUser(currentUser)){
          this.userService.listAccountsByUserID(currentUser.UserID).subscribe(result => {
            if (result == null || result.length == 0) {
              this.router.navigate(['/']);
              return;
            }
            this.router.navigate([`/landowner-dashboard/${result[0].AccountNumber}`]);
          });
        } else{
          this.router.navigate(['/']);
        }
    });
  }

  ngOnDestroy(): void {
    this.watchUserChangeSubscription.unsubscribe();
  }
}
