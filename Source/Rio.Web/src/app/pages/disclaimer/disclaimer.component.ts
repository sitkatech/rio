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
  private return : string = '';
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
    this.route.queryParams.subscribe(params => this.return =  params['return'] || '/');
    this.forced = this.route.snapshot.paramMap.get("forced") === "true";
  }

  public checkDisclaimerAcknowledged(): boolean {
      return !this.currentUser || (!this.forced && this.currentUser.DisclaimerAcknowledgedDate != null) ? false : true;
  }

  public setDisclaimerAcknowledged(): void {
    this.userService.setDisclaimerAcknowledgedDate(this.currentUser.UserID).subscribe(x=>{
      this.authenticationService.refreshUserInfo(x);
      this.router.navigate([this.return]);
    });
  }
}
