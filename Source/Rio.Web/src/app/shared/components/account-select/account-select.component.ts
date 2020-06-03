import { Component, OnInit, ViewEncapsulation, ViewChild, HostListener, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AccountSimpleDto } from '../../models/account/account-simple-dto';
import { UserDto } from '../../models';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

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
  public searchText: string;
  public modalReference: NgbModalRef;

  constructor(private authenticationService: AuthenticationService,
              private modalService: NgbModal) { }

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
  }

  ngOnDestroy(): void {
    this.watchUserChangeSubscription.unsubscribe();
  }

  public isUnassignedOrDisabled(): boolean {
    return this.authenticationService.isUserUnassigned(this.currentUser) || this.authenticationService.isUserRoleDisabled(this.currentUser);
  }

  public getAvailableAccounts(): Array<AccountSimpleDto> {
    return this.authenticationService.getAvailableAccounts()
  }

  public getFilteredAccounts(): Array<AccountSimpleDto> {
    return this.getAvailableAccounts()?.filter(x => (this.searchText == null || x.ShortAccountDisplayName.includes(this.searchText)) && x != this.activeAccount);
  }

  public setCurrentAccount(): void {
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }
    if (this.selectedAccount) {
    this.authenticationService.setActiveAccount(this.selectedAccount);
    this.activeAccount = this.selectedAccount;
    }
  }

  public compareAccountsFn(c1: AccountSimpleDto, c2: AccountSimpleDto): boolean {
    return c1 && c2 ? c1.AccountID === c2.AccountID : c1 === c2;
  }

  public showDropdown(): boolean {
    const accountList = this.getAvailableAccounts();
    return accountList && accountList.length > 1;
  }

  public launchModal(modalContent: any) {
    this.modalReference = this.modalService.open(modalContent, { windowClass: 'modal-size', ariaLabelledBy: 'selectAccountModalTitle', backdrop: 'static', keyboard: false });
    this.modalReference.result.then((result) => {
      this.searchText = null;
      this.selectedAccount = null;
    }, (reason) => {
      this.searchText = null;
      this.selectedAccount = null;
    });
  }
}
