import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserService } from 'src/app/services/user/user.service';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AccountDto } from 'src/app/shared/generated/model/account-dto';
import { AccountIncludeParcelsDto } from 'src/app/shared/generated/model/account-include-parcels-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';

@Component({
  selector: 'rio-water-accounts-list',
  templateUrl: './water-accounts-list.component.html',
  styleUrls: ['./water-accounts-list.component.scss']
})
export class WaterAccountsListComponent implements OnInit {
  public watchUserChangeSubscription: any;
  public currentUser: UserDto;
  public currentPage: number =  1;

  public currentUserAccounts: AccountIncludeParcelsDto[];
  public loadingAccounts: boolean =  false;

  public modalReference: NgbModalRef;

  public accountToRemove: AccountDto;

  constructor(private authenticationService: AuthenticationService,
    private userService: UserService,
    private alertService: AlertService,
    private router: Router,
    private modalService: NgbModal
    ) { }

  ngOnInit(): void {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.loadingAccounts = true;
      this.userService.listAccountsIncludeParcelsByUserID(this.currentUser.UserID).subscribe(userAccounts => {
        this.currentUserAccounts = userAccounts;
        this.loadingAccounts = false;
      });
    });
  }

  public currentUserIsAdmin() {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }

  public launchModal(modalContent: any, accountToRemove: AccountDto) {
    this.accountToRemove = accountToRemove;
    this.modalReference = this.modalService.open(modalContent, { ariaLabelledBy: 'removeAccountModalContent', backdrop: 'static', keyboard: false });
  }

  public removeAccountToRemove() {
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }
    this.userService.removeAccountByIDForCurrentUser(this.accountToRemove.AccountID).subscribe(response => {
      this.authenticationService.refreshUserInfo(this.currentUser);
      this.alertService.pushAlert(new Alert(`Account #${this.accountToRemove.AccountNumber} (${this.accountToRemove.AccountName}) successfully removed from accounts you manage.`, AlertContext.Success));
      this.accountToRemove = null;
    })
  }
}
