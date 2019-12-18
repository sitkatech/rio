import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AccountDto } from 'src/app/shared/models/account/account-dto';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/services/user/user.service';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from 'src/app/shared/models';
import { AccountService } from 'src/app/services/account/account.service';

@Component({
  selector: 'rio-account-list',
  templateUrl: './account-list.component.html',
  styleUrls: ['./account-list.component.scss']
})
export class AccountListComponent implements OnInit {

  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public accounts: Array<AccountDto>

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private accountService: AccountService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.accountService.listAllAccounts().subscribe(accounts =>{
        this.accounts = accounts;
      })
    });
  }

}
