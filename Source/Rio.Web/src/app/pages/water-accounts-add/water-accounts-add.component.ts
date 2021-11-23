import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account/account.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserService } from 'src/app/services/user/user.service';
import { AccountDto } from 'src/app/shared/generated/model/account-dto';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { UserEditAccountsDto } from 'src/app/shared/generated/model/user-edit-accounts-dto';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { RoleEnum } from 'src/app/shared/models/enums/role.enum';
import { AlertService } from 'src/app/shared/services/alert.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'rio-water-accounts-add',
  templateUrl: './water-accounts-add.component.html',
  styleUrls: ['./water-accounts-add.component.scss']
})
export class WaterAccountsAddComponent implements OnInit {

  private watchUserChangeSubscription: any;
  public currentUser: UserDto;

  public verificationKeyToSearchFor: string;
  public accountsToAdd: Array<AccountDto> = new Array<AccountDto>();
  public currentUserAccounts: Array<AccountSimpleDto> = new Array<AccountSimpleDto>();
  
  public searchingForAccount: boolean = false;
  public accountNotFound: boolean = false;
  public accountAlreadyPresentInList: boolean = false;
  public userAlreadyHasAccessToAccount: boolean = false;
  public accountSuccessfullyFoundAndAddedToList: boolean = false;
  public lastSuccessfullyAddedAccountVerificationKey: string;
  
  public confirmedFullNameForRegistration: string;

  public introRichText: number = CustomRichTextType.WaterAccountsAdd;
  public legalText: number = CustomRichTextType.WaterAccountsAddLegalText;

  isLoadingSubmit: boolean;
  updatingUserRole: boolean;

  constructor(
    private authenticationService: AuthenticationService,
    private accountService: AccountService,
    private userService: UserService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private alertService: AlertService
  ) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      
      this.userService.listAccountsByUserID(this.currentUser.UserID).subscribe(userAccounts => {
        this.currentUserAccounts = userAccounts;
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }


  public findAccount() {
    if (this.verificationKeyToSearchFor == null || this.verificationKeyToSearchFor == undefined) {
      return null;
    }

    if (this.accountsToAdd.length > 0 && this.accountsToAdd.some(x => x.AccountVerificationKey == this.verificationKeyToSearchFor)) {
      this.accountAlreadyPresentInList = true;
      return;
    }

    if (!this.currentUserIsAdmin() && this.currentUserAccounts.length > 0 && this.currentUserAccounts.some(x => x.AccountVerificationKey == this.verificationKeyToSearchFor)) {
      this.userAlreadyHasAccessToAccount =  true;
      return;
    }

    this.turnOffAccountSearchErrors();   
    this.accountSuccessfullyFoundAndAddedToList = false;
    
    this.searchingForAccount = true;
    this.accountService.getAccountByAccountVerificationKey(this.verificationKeyToSearchFor).subscribe(accountDto => {
      this.searchingForAccount = false;
      this.lastSuccessfullyAddedAccountVerificationKey = this.verificationKeyToSearchFor;
      this.accountSuccessfullyFoundAndAddedToList = true;
      this.verificationKeyToSearchFor = null;
      this.accountsToAdd.push(accountDto);
      this.cdr.detectChanges();
    },
      error => {
        if (error.status == 404) {
          this.accountNotFound = true;
        };
        this.searchingForAccount = false;
        this.cdr.detectChanges();
      });
  }

  public currentUserIsAdmin() {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }

  public turnOffAccountSearchErrors() {
    this.accountAlreadyPresentInList = false;
    this.accountNotFound = false;
    this.userAlreadyHasAccessToAccount = false;
  }

  public isFullNameConfirmedForRegistration(): boolean {
    if(this.confirmedFullNameForRegistration)
    {
      return this.confirmedFullNameForRegistration.toLowerCase() === this.currentUser.FullName.toLowerCase();
    }
    return false;
  }

  public registerAccounts(): void {
    this.isLoadingSubmit = true;

    let userEditAccountsDto = new UserEditAccountsDto();
    userEditAccountsDto.AccountIDs = this.accountsToAdd.map(x => x.AccountID);

    this.updatingUserRole = this.currentUser.Role.RoleID == RoleEnum.Unassigned;
    this.userService.addAccountsToCurrentUser(userEditAccountsDto).subscribe(user => {
      this.isLoadingSubmit = false;
      this.router.navigateByUrl(`/water-accounts`).then(x => {
        this.alertService.pushAlert(new Alert(`Water Account(s) successfully added!`, AlertContext.Success));
        if (this.updatingUserRole) {
          this.alertService.pushAlert(new Alert(`Congratulations! You have completed sign-up for the ${environment.platformShortName}`, AlertContext.Success));
        }
      });
    }, error=>{
      this.isLoadingSubmit = false;
      this.cdr.detectChanges();
    });
  }
}