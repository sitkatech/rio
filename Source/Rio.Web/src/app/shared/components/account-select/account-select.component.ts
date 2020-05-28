import { Component, OnInit, ViewEncapsulation, ViewChild, HostListener, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AccountSimpleDto } from '../../models/account/account-simple-dto';
import { UserDto } from '../../models';
import { SelectDropDownComponent } from 'ngx-select-dropdown';

declare var jQuery: any;

@Component({
  selector: 'rio-account-select',
  templateUrl: './account-select.component.html',
  styleUrls: ['./account-select.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AccountSelectComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  public activeAccount: AccountSimpleDto;
  public selectedAccount: any;
  public currentUser: UserDto;
  @ViewChild("accountsDropdown") accountsDropdown: SelectDropDownComponent;

  public accountDropdownConfig = {
    search: true,
    searchPlaceholder: "Search Account...",
    height: '320px',
    placeholder: "Switch account",
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
    })
    
    jQuery(document).on("show.bs.dropdown", function(event){
        if (this.accountsDropdown && this.accountsDropdown.toggleDropdown) {
          this.accountsDropdown.toggleSelectDropdown();
        }
    }.bind(this));
  }

  ngOnDestroy(): void {
    jQuery(document).off("show.bs.dropdown")
  }

  public isUnassignedOrDisabled(): boolean {
    return this.authenticationService.isUserUnassigned(this.currentUser) || this.authenticationService.isUserRoleDisabled(this.currentUser);
  }

  public getAvailableAccounts(): Array<AccountSimpleDto> {
    return this.authenticationService.getAvailableAccounts();
  }

  public setCurrentAccount(): void {
    if (this.selectedAccount) {
    this.authenticationService.setActiveAccount(this.selectedAccount);
    this.activeAccount = this.selectedAccount;
    this.accountsDropdown.deselectItem(this.selectedAccount, 0);
    }
  }

  public compareAccountsFn(c1: AccountSimpleDto, c2: AccountSimpleDto): boolean {
    return c1 && c2 ? c1.AccountID === c2.AccountID : c1 === c2;
  }

  public showDropdown(): boolean {
    const accountList = this.getAvailableAccounts();
    return accountList && accountList.length > 1;
  }
}
