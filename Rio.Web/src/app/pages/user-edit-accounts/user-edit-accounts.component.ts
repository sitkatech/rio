import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserService } from 'src/app/services/user/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AccountService } from 'src/app/services/account/account.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { Alert } from 'src/app/shared/models/alert';
import { RoleEnum } from 'src/app/shared/generated/enum/role-enum';
import { AccountDto } from 'src/app/shared/generated/model/account-dto';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { UserEditAccountsDto } from 'src/app/shared/generated/model/user-edit-accounts-dto';

@Component({
  selector: 'rio-user-edit-accounts',
  templateUrl: './user-edit-accounts.component.html',
  styleUrls: ['./user-edit-accounts.component.scss']
})
export class UserEditAccountsComponent implements OnInit, OnDestroy {
  public watchAccountChangeSubscription: any;
  public currentUser: UserDto;
  public userID: number;
  public user: UserDto;
  public allAccounts: AccountDto[];
  public accountsToSave: AccountSimpleDto[];

  public accountDropdownConfig = {
    search: true,
    height: '320px',
    placeholder: "Select a account from the list of accounts",
    displayKey: "AccountDisplayName",
    searchOnKey: "AccountDisplayName",
  };

  public selectedAccount: AccountDto[];
  public filteredAccounts: AccountDto[];

  public isLoadingSubmit: boolean = false;

  constructor(
    private authenticationService: AuthenticationService,
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute,
    private accountService: AccountService,
    private alertService: AlertService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.allAccounts = new Array<AccountDto>();
    this.watchAccountChangeSubscription = this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;

      this.userID = parseInt(this.route.snapshot.paramMap.get("id"));

      forkJoin(
        this.userService.getUserFromUserID(this.userID),
        this.accountService.listAllAccounts(),
        this.userService.listAccountsByUserID(this.userID)
      ).subscribe(([user, accounts, userAccounts]) => {

        this.user = user;
        if (user.Role.RoleID == RoleEnum.Admin){
          this.alertService.pushAlert(new Alert("Oops! Looks like you typed or copied and pasted a URL that doesn't quite work. That feature may have been removed or disabled."));
          this.router.navigate(["/"]);
        }
        this.allAccounts = accounts;
        this.accountsToSave = userAccounts;

        this.filteredAccounts = this.allAccounts.filter(x => !this.accountsToSave.map(y => y.AccountID).includes(x.AccountID));
      });
    });
  }

  ngOnDestroy() {
    this.watchAccountChangeSubscription.unsubscribe();
    
    this.cdr.detach();
  }

  public removeAccount(account: AccountSimpleDto): void {
    const index = this.accountsToSave.indexOf(account);
    if (index > -1) {
      this.accountsToSave.splice(index, 1);
    }

    this.filteredAccounts = this.allAccounts.filter(x => !this.accountsToSave.map(y => y.AccountID).includes(x.AccountID));
  }

  public addAccount(account: AccountSimpleDto): void {
    this.accountsToSave.push(account);

    this.selectedAccount = null;
    this.filteredAccounts = this.allAccounts.filter(x => !this.accountsToSave.map(y => y.AccountID).includes(x.AccountID));
  }

  public onSubmit(form: HTMLFormElement): void {
    this.isLoadingSubmit = true;

    let userEditAccountsDto = new UserEditAccountsDto();
    userEditAccountsDto.AccountIDs = this.accountsToSave.map(x => x.AccountID);


    this.userService.editAccounts(this.userID, userEditAccountsDto).subscribe(user => {
      this.isLoadingSubmit = false;
      form.reset();
      this.router.navigateByUrl(`/users/${this.userID}`).then(x => {
        this.alertService.pushAlert(new Alert(`The associated accounts for User ${this.user.FullName} were successfully updated.`, AlertContext.Success));
      });
    }, error=>{
      this.isLoadingSubmit = false;
      this.cdr.detectChanges();
    });
  }
}
