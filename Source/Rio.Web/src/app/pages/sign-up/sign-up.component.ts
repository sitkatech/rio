import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from 'src/app/shared/models';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {

  private watchUserChangeSubscription: any;
  public currentUser: UserDto;
  
  public introRichText : number = CustomRichTextType.SignUp;
  public stepOneRichText : number = CustomRichTextType.SignUpStepOne;
  public stepTwoRichText : number = CustomRichTextType.SignUpStepTwo;
  public stepThreeRichText : number = CustomRichTextType.SignUpStepThree;

  constructor(
    private authenticationService : AuthenticationService
  ) { }

  ngOnInit() {
      this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => { 
        this.currentUser = currentUser;
    });
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
