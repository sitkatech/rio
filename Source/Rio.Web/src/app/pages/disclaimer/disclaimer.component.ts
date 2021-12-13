import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/services/user/user.service';
import { AuthenticationService } from 'src/app/services/authentication.service'
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { UserDto } from 'src/app/shared/generated/model/user-dto';

@Component({
  selector: 'rio-disclaimer',
  templateUrl: './disclaimer.component.html',
  styleUrls: ['./disclaimer.component.scss']
})
export class DisclaimerComponent implements OnInit {

  private watchUserChangeSubscription : any;
  private currentUser : UserDto;
  private forced : boolean = true;
  private returnRoute : string = '';
  public richTextTypeID: number = CustomRichTextType.Disclaimer;
  returnQueryParams: any;

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
    this.route.queryParams.subscribe(params => {
      this.returnRoute =  params['route'] || '/';
      this.returnQueryParams = params['queryParams'] || null;
    });
    this.forced = this.route.snapshot.paramMap.get("forced") === "true";
  }

  public checkDisclaimerAcknowledged(): boolean {
      return !this.currentUser || (!this.forced && this.currentUser.DisclaimerAcknowledgedDate != null) ? false : true;
  }

  public setDisclaimerAcknowledged(): void {
    this.userService.setDisclaimerAcknowledgedDate(this.currentUser.UserID).subscribe(x=>{
      this.authenticationService.refreshUserInfo(x);
      this.router.navigate([this.returnRoute], {queryParams : JSON.parse(this.returnQueryParams)});
    });
  }
}
