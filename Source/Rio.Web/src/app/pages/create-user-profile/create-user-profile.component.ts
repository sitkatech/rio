import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from 'src/app/shared/models';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-create-user-profile',
  templateUrl: './create-user-profile.component.html',
  styleUrls: ['./create-user-profile.component.scss']
})
export class SignUpComponent implements OnInit {

  private watchUserChangeSubscription: any;
  public currentUser: UserDto;
  
  public introRichText : number = CustomRichTextType.CreateUserProfile;
  public stepOneRichText : number = CustomRichTextType.CreateUserProfileStepOne;
  public stepTwoRichText : number = CustomRichTextType.CreateUserProfileStepTwo;
  public stepThreeRichText : number = CustomRichTextType.CreateUserProfileStepThree;

  constructor(
    private authenticationService : AuthenticationService
  ) { }

  ngOnInit() {
      this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => { 
        this.currentUser = currentUser;
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
  }

  public isCurrentUserNullOrUndefined(): boolean {
    return this.authenticationService.isCurrentUserNullOrUndefined(); 
  }

  public isCurrentUserDisabled(): boolean {
    return this.authenticationService.isCurrentUserDisabled();
  }

  public isCurrentUserLandownerDemoUserOrAdmin() : boolean {
    return this.authenticationService.isCurrentUserALandOwnerOrDemoUserOrAdministrator();
  }

  public createAccount(): void{
    this.authenticationService.createAccount();
  }

}
