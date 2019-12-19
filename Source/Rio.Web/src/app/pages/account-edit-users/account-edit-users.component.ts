import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AccountService } from 'src/app/services/account/account.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountDto, AccountEditUsersDto } from 'src/app/shared/models/account/account-dto';
import { forkJoin } from 'rxjs';
import { UserService } from 'src/app/services/user/user.service';
import { UserDto } from 'src/app/shared/models';
import { UserSimpleDto } from 'src/app/shared/models/user/user-simple-dto';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { Alert } from 'src/app/shared/models/alert';

@Component({
  selector: 'rio-account-edit-users',
  templateUrl: './account-edit-users.component.html',
  styleUrls: ['./account-edit-users.component.scss']
})
export class AccountEditUsersComponent implements OnInit, OnDestroy {
  public watchUserChangeSubscription: any;
  public currentUser: import("c:/git/sitkatech/rio/Source/Rio.Web/src/app/shared/models/index").UserDto;
  public accountID: number;
  public account: AccountDto;
  public allUsers: UserDto[];
  public usersToSave: UserSimpleDto[];

  public userDropdownConfig = {
    search: true,
    height: '320px',
    placeholder: "Select a user from the list of users",
    displayKey: "FullName",
    searchOnKey: "FullName",
  };

  public selectedUser: UserDto[];
  public filteredUsers: UserDto[];

  public isLoadingSubmit: boolean = false;

  constructor(
    private authenticationService: AuthenticationService,
    private accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute,
    private userService: UserService,
    private alertService: AlertService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.allUsers = new Array<UserDto>();
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.accountID = parseInt(this.route.snapshot.paramMap.get("id"));

      forkJoin(
        this.accountService.getAccountByID(this.accountID),
        this.userService.getUsers()
      ).subscribe(([account, users]) => {
        this.account = account;
        this.allUsers = users;
        this.usersToSave = account.Users;

        this.filteredUsers = this.allUsers.filter(x => !this.usersToSave.map(y => y.UserID).includes(x.UserID));
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public removeUser(user: UserSimpleDto): void {
    const index = this.usersToSave.indexOf(user);
    if (index > -1) {
      this.usersToSave.splice(index, 1);
    }

    this.filteredUsers = this.allUsers.filter(x => !this.usersToSave.map(y => y.UserID).includes(x.UserID));
  }

  public addUser(user: UserSimpleDto): void {
    this.usersToSave.push(user);

    this.selectedUser = null;
    this.filteredUsers = this.allUsers.filter(x => !this.usersToSave.map(y => y.UserID).includes(x.UserID));
  }

  public onSubmit(form: HTMLFormElement): void {
    this.isLoadingSubmit = true;

    let accountEditUsersDto = new AccountEditUsersDto();
    accountEditUsersDto.UserIDs = this.usersToSave.map(x => x.UserID);


    this.accountService.editUsers(this.accountID, accountEditUsersDto).subscribe(account => {
      this.isLoadingSubmit = false;
      form.reset();
      this.router.navigateByUrl(`/accounts/${this.accountID}`).then(x => {
        this.alertService.pushAlert(new Alert(`The associated users for Account ${this.account.AccountNumber} were successfully updated.`, AlertContext.Success));
      });
    }, error=>{
      this.isLoadingSubmit = false;
      this.cdr.detectChanges();
    });
  }
}
