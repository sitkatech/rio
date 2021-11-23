import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserService } from 'src/app/services/user/user.service';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { UserPartnerInviteDto } from 'src/app/shared/generated/model/user-partner-invite-dto';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { AlertService } from 'src/app/shared/services/alert.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'rio-water-accounts-invite',
  templateUrl: './water-accounts-invite.component.html',
  styleUrls: ['./water-accounts-invite.component.scss']
})
export class WaterAccountsInviteComponent implements OnInit {
  public introRichText: number = CustomRichTextType.WaterAccountsInvite;

  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public model: UserPartnerInviteDto;
  public isLoadingSubmit: boolean = false;
  public loadingAccounts: boolean =  false;
  public currentUserAccounts: AccountSimpleDto[];

  public selectedAccountIDs: number[];
  public accountSelectError

  constructor(private cdr: ChangeDetectorRef,
    private userService: UserService,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private router: Router) { }

  ngOnInit(): void {
    this.model = new UserPartnerInviteDto();
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.loadingAccounts = true;
      this.userService.listAccountsByUserID(currentUser.UserID).subscribe(accounts => {
        this.loadingAccounts = false;
        this.currentUserAccounts = accounts;
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  canInviteUser(): boolean {
    return this.model.FirstName && this.model.LastName && this.model.Email && this.model.Email.indexOf("@") != -1;
  }

  updateSelectedAccountIDs(selectedID: number) {
    this.accountSelectError = false;
    if (!this.selectedAccountIDs) {
      this.selectedAccountIDs = new Array<number>();
    }

    if (this.selectedAccountIDs.some(x => x == selectedID)) {
      this.selectedAccountIDs = this.selectedAccountIDs.filter(x => x != selectedID);
      if (this.selectedAccountIDs.length == 0) {
        this.accountSelectError = true;
      }
      return;
    }

    this.selectedAccountIDs.push(selectedID);
  }

  onSubmit(inviteUserForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;

    if (this.inviteUserFormIsValid(inviteUserForm)) {
      this.model.AccountIDs = this.selectedAccountIDs;
      this.userService.invitePartner(this.model)
        .subscribe(response => {
          this.isLoadingSubmit = false;
          window.scrollTo(0, 0);
          this.router.navigate(['/water-accounts']).then(() => {
            this.alertService.pushAlert(new Alert(`An invitation email has been sent to ${this.model.Email}`, AlertContext.Success));
          })
        }
          ,
          error => {
            this.isLoadingSubmit = false;
            window.scrollTo(0, 0);
            this.cdr.detectChanges();
          }
        );
    }
    else {
      this.isLoadingSubmit = false;
      window.scrollTo(0, 0);
      this.alertService.pushAlert(new Alert("The form contains errors. Please address the errors and try submitting again.", AlertContext.Danger));
      Object.keys(inviteUserForm.form.controls).forEach(key => {
        inviteUserForm.form.get(key).markAsTouched();
      });
    }
  }

  public submitButtonDisabled(inviteUserForm: HTMLFormElement): boolean {
    if (this.isLoadingSubmit || this.accountSelectError) {
      return true;
    }

    let disabled = false;
    Object.keys(inviteUserForm.form.controls).some(key => {
      let control = inviteUserForm.form.get(key);
      if (control.touched && !control.valid) {
        disabled = true;
        return true;
      };
    });

    return disabled;
  }

  public inviteUserFormIsValid(inviteUserForm: HTMLFormElement): boolean {
    var accountIDsAreValid = this.selectedAccountIDs != null && this.selectedAccountIDs != undefined && this.selectedAccountIDs.length > 0;
    this.accountSelectError = !accountIDsAreValid;
    return inviteUserForm.form.valid && accountIDsAreValid;
  }

  public platformShortName(): string {
    return environment.platformShortName;
  }

}
