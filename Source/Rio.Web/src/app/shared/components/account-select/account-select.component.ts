import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AccountSimpleDto } from '../../models/account/account-simple-dto';
import { UserDto } from '../../models';

@Component({
  selector: 'rio-account-select',
  templateUrl: './account-select.component.html',
  styleUrls: ['./account-select.component.scss']
})
export class AccountSelectComponent implements OnInit {
  private watchUserChangeSubscription: any;
  public activeAccount: AccountSimpleDto;
  public currentUser: UserDto;

  public accountDropdownConfig = {
    search: true,
    height: '320px',
    placeholder: "Select an account",
    displayKey: "ShortAccountDisplayName",
    searchOnKey: "AccountDisplayName",
  }

  constructor(private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;

      // do not attempt any API hits if the user is known to be unassigned.
      if (currentUser && !this.isUnassignedOrDisabled()) {

        // display the correct active account in the dropdown below the username.
        // on pages which need to react to the active account, include this call in ngInit and put reactive logic in the subscribe statement.
        this.authenticationService.getActiveAccount().subscribe((account: AccountSimpleDto) => { this.activeAccount = account; });
      }
    });
  }

  public isUnassignedOrDisabled(): boolean {
    return this.authenticationService.isUserUnassigned(this.currentUser) || this.authenticationService.isUserRoleDisabled(this.currentUser);
  }

  public getAvailableAccounts(): Array<AccountSimpleDto> {
    return this.authenticationService.getAvailableAccounts();
  }

  public setCurrentAccount(): void {
    this.authenticationService.setActiveAccount(this.activeAccount);
  }

  public compareAccountsFn(c1: AccountSimpleDto, c2: AccountSimpleDto): boolean {
    return c1 && c2 ? c1.AccountID === c2.AccountID : c1 === c2;
  }

  public showDropdown(): boolean{
    return this.getAvailableAccounts().length > 1;
  }
}
