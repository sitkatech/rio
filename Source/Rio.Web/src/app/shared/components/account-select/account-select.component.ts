import { Component, OnInit, ViewEncapsulation, ViewChild, HostListener, OnDestroy, ChangeDetectorRef, Input } from '@angular/core';
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
  @Input() linkText = "Click here to change your selected account";
  
  private watchUserChangeSubscription: any;
  private watchAccountChangeSubscription: any;
  public activeAccount: AccountSimpleDto;
  public selectedAccount: any;
  public currentUser: UserDto;
  public searchText: string;
  public modalReference: NgbModalRef;

  constructor(private authenticationService: AuthenticationService,
              private modalService: NgbModal,
              private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
    })

    this.watchAccountChangeSubscription = this.authenticationService.getActiveAccount().subscribe(account => { 
      if (account) {
        this.activeAccount = account;
        this.cdr.detectChanges();            
      }
    });
  }

  ngOnDestroy(): void {
    this.watchUserChangeSubscription.unsubscribe();
    this.watchAccountChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
  }

  public getActiveAccountName() : string {
    return this.activeAccount ? this.activeAccount.ShortAccountDisplayName : "No account selected";
  }

  public isUnassignedOrDisabled(): boolean {
    return this.authenticationService.isUserUnassigned(this.currentUser) || this.authenticationService.isUserRoleDisabled(this.currentUser);
  }

  public getAvailableAccounts(): Array<AccountSimpleDto> {
    return this.authenticationService.getAvailableAccounts()
  }

  public getFilteredAccounts(): Array<AccountSimpleDto> {
    return this.getAvailableAccounts()?.filter(x => (this.searchText == null || x.ShortAccountDisplayName.includes(this.searchText)) && x.AccountID != this.activeAccount?.AccountID);
  }

  public setCurrentAccount(): void {
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }
    if (this.selectedAccount) {
      this.authenticationService.setActiveAccount(this.selectedAccount, false);
    }
  }

  public compareAccountsFn(c1: AccountSimpleDto, c2: AccountSimpleDto): boolean {
    return c1 && c2 ? c1.AccountID === c2.AccountID : c1 === c2;
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
