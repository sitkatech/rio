import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/services/user/user.service';
import { AuthenticationService } from 'src/app/services/authentication.service'
import { UserDto } from 'src/app/shared/models';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-disclaimer',
  templateUrl: './disclaimer.component.html',
  styleUrls: ['./disclaimer.component.scss']
})
export class DisclaimerComponent implements OnInit {

  private watchUserChangeSubscription : any;
  private currentUser : UserDto;
  private forced : boolean = true;
  public richTextTypeID: number = CustomRichTextType.Disclaimer;

  constructor(
    private userService: UserService,
    private authenticationService : AuthenticationService,
    private router : Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
    });
    this.forced = this.route.snapshot.paramMap.get("forced") === "true";
  }

  public checkDisclaimerAcknowledged(): boolean {
      return !this.currentUser || (!this.forced && this.currentUser.DisclaimerAcknowledgedDate != null) ? false : true;
  }

  public setDisclaimerAcknowledged(): void {
    this.userService.setDisclaimerAcknowledgedDate(this.currentUser.UserID).subscribe(x=>{
      this.authenticationService.refreshUserInfo(x);
      if (this.authenticationService.isUserAnAdministrator(x)){
        this.router.navigate(['/manager-dashboard']);
      } else if (this.authenticationService.isUserALandOwnerOrDemoUser(x)){
        this.router.navigate(['/landowner-dashboard']);
      } else{
        this.router.navigate(['/']);
      }
    });
  }

}
