import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';

@Component({
  selector: 'rio-create-user-profile',
  templateUrl: './create-user-profile.component.html',
  styleUrls: ['./create-user-profile.component.scss']
})
export class CreateUserProfileComponent implements OnInit {

  
  public currentUser: UserDto;
  
  public introRichText : number = CustomRichTextTypeEnum.CreateUserProfile;
  public stepOneRichText : number = CustomRichTextTypeEnum.CreateUserProfileStepOne;
  public stepTwoRichText : number = CustomRichTextTypeEnum.CreateUserProfileStepTwo;
  public stepThreeRichText : number = CustomRichTextTypeEnum.CreateUserProfileStepThree;

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
