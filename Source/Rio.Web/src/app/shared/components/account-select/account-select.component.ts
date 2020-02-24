import { Component, OnInit, ViewEncapsulation, ViewChild } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AccountSimpleDto } from '../../models/account/account-simple-dto';
import { UserDto } from '../../models';
import { SelectDropDownComponent } from 'ngx-select-dropdown';

@Component({
  selector: 'rio-account-select',
  templateUrl: './account-select.component.html',
  styleUrls: ['./account-select.component.scss'],
  encapsulation: ViewEncapsulation.None,
  host: {
    '(document:click)': 'onClick($event)',
  }
})
export class AccountSelectComponent implements OnInit {
  private watchUserChangeSubscription: any;
  public activeAccount: AccountSimpleDto;
  public currentUser: UserDto;
  @ViewChild("accountsDropdown", { static: false }) accountsDropdown: SelectDropDownComponent;

  public accountDropdownConfig = {
    search: true,
    searchPlaceholder: "Search Account...",
    height: '320px',
    placeholder: "Select an account",
    displayKey: "ShortAccountDisplayName",
    searchOnKey: "ShortAccountDisplayName",
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

  public showDropdown(): boolean {
    const accountList = this.getAvailableAccounts();
    return accountList && accountList.length > 1;
  }

  // NP Poor man's hack to make sure the account dropdown closes when it loses focus, like a dropdown should. If I'd known that ngx-select-dropdown was missing this I would have picked a different component, but here we are.
  public onClick(event) {
    if (!(event.target.classList.contains("ngx-dropdown-button") || event.target.parentElement.classList.contains("ngx-dropdown-button"))) {
      if (this.accountsDropdown.toggleDropdown) {
        this.accountsDropdown.toggleSelectDropdown();
      }
    }
  }

}
