import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { forkJoin } from 'rxjs';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { AccountStatusService } from 'src/app/services/accountStatus/account-status.service';
import { AccountService } from 'src/app/services/account/account.service';
import { AccountDto } from 'src/app/shared/generated/model/account-dto';
import { AccountStatusDto } from 'src/app/shared/generated/model/account-status-dto';
import { AccountUpdateDto } from 'src/app/shared/generated/model/account-update-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';

@Component({
  selector: 'rio-account-edit',
  templateUrl: './account-edit.component.html',
  styleUrls: ['./account-edit.component.scss']
})
export class AccountEditComponent implements OnInit {
  
  private currentUser: UserDto;

  public accountID: number;
  public account: AccountDto;
  public model: AccountUpdateDto;
  public accountStatuses: Array<AccountStatusDto>;
  public isLoadingSubmit: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authenticationService: AuthenticationService,
    private accountService: AccountService,
    private accountStatusService: AccountStatusService,
    private cdr: ChangeDetectorRef,
    private alertService: AlertService
  ) {
  }

  ngOnInit() {
    this.model = new AccountUpdateDto();
    
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;

      if (!this.authenticationService.isUserAnAdministrator(this.currentUser)) {
        this.router.navigateByUrl("/not-found")
          .then();
        return;
      }

      this.accountID = parseInt(this.route.snapshot.paramMap.get("id"));

      forkJoin(
        this.accountService.getAccountByID(this.accountID),
        this.accountStatusService.getAccountStatuses()
      ).subscribe(([account, accountStatuses]) => {
        this.account = account instanceof Array
          ? null
          : account as AccountDto;

        this.accountStatuses = accountStatuses.sort((a: AccountStatusDto, b: AccountStatusDto) => {
          if (a.AccountStatusDisplayName > b.AccountStatusDisplayName)
            return 1;
          if (a.AccountStatusDisplayName < b.AccountStatusDisplayName)
            return -1;
          return 0;
        });

        this.model.AccountName = this.account.AccountName;
        this.model.Notes = this.account.Notes;
        this.model.AccountStatusID = account.AccountStatus.AccountStatusID;

        this.cdr.detectChanges();
      });
    });
  }

  ngOnDestroy() {
    
    
    this.cdr.detach();
  }

  onSubmit(editAccountForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;

    this.accountService.updateAccount(this.accountID, this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        this.router.navigateByUrl("/accounts/" + this.accountID).then(x => {
          this.alertService.pushAlert(new Alert(`The account ${this.model.AccountName} was successfully updated.`, AlertContext.Success));
        });
      }
        ,
        error => {
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }

}
