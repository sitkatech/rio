import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-create-user-profile',
  templateUrl: './create-user-profile.component.html',
  styleUrls: ['./create-user-profile.component.scss']
})
export class CreateUserProfileComponent implements OnInit {

  
  public currentUser: UserDto;
  
  public introRichText : number = CustomRichTextType.CreateUserProfile;
  public stepOneRichText : number = CustomRichTextType.CreateUserProfileStepOne;
  public stepTwoRichText : number = CustomRichTextType.CreateUserProfileStepTwo;
  public stepThreeRichText : number = CustomRichTextType.CreateUserProfileStepThree;

  constructor(
    private authenticationService : AuthenticationService
  ) { }

  ngOnInit() {
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
