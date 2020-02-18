import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/services/user/user.service';
import { AuthenticationService } from 'src/app/services/authentication.service'
import { UserDto } from 'src/app/shared/models';

@Component({
  selector: 'rio-disclaimer',
  templateUrl: './disclaimer.component.html',
  styleUrls: ['./disclaimer.component.scss']
})
export class DisclaimerComponent implements OnInit {

  private watchUserChangeSubscription : any;
  private currentUser : UserDto;

  constructor(
    private userService: UserService,
    private authenticationService : AuthenticationService,
    private router : Router
  ) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
    });
  }

  public setDisclaimerAcknowledged(): void {
    this.userService.setDisclaimerAcknowledgedDate(this.currentUser.UserID).subscribe(x=>{
      this.authenticationService.refreshUserInfo(x);
      if (this.authenticationService.isUserAnAdministrator(x)){
        this.router.navigate(['/manager-dashboard']);
      } else if (this.authenticationService.isUserALandOwner(x)){
        this.router.navigate(['/landowner-dashboard']);
      } else{
        this.router.navigate(['/']);
      }
    });
  }

}
