import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AccountService } from 'src/app/services/account/account.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountDto } from 'src/app/shared/models/account/account-dto';
import { forkJoin } from 'rxjs';
import { UserService } from 'src/app/services/user/user.service';
import { UserDto } from 'src/app/shared/models';

@Component({
  selector: 'rio-account-edit-users',
  templateUrl: './account-edit-users.component.html',
  styleUrls: ['./account-edit-users.component.scss']
})
export class AccountEditUsersComponent implements OnInit {
  public watchUserChangeSubscription: any;
  public currentUser: import("c:/git/sitkatech/rio/Source/Rio.Web/src/app/shared/models/index").UserDto;
  public accountID: number;
  public account: AccountDto;
  public users: UserDto[];

  public userDropdownConfig = {
    search: true,
    height: '320px',
    placeholder: "Select a user from the list of users",
    displayKey: "FullName",
    searchOnKey: "FullName",
  };

  public selectedUser: any;

  constructor(
    private authenticationService: AuthenticationService,
    private accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute,
    private userService: UserService
  ) { }

  ngOnInit() {
    this.users = new Array<UserDto>();
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.accountID = parseInt(this.route.snapshot.paramMap.get("id"));

      forkJoin(
        this.accountService.getAccountByID(this.accountID),
        this.userService.getUsers()
      ).subscribe(([account, users]) => {
        this.account = account;
        this.users = users;
      });


    });
  }

  public selected() : void {
    console.log(this.selectedUser);
  }
}
