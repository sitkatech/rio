import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AccountUpdateDto as AccountUpsertDto } from 'src/app/shared/models/account/account-update-dto';
import { AccountService } from 'src/app/services/account/account.service';
import { Router } from '@angular/router';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { AccountStatusService } from 'src/app/services/accountStatus/account-status.service';
import { AccountStatusDto } from 'src/app/shared/models/account/account-status-dto';

@Component({
  selector: 'rio-account-new',
  templateUrl: './account-new.component.html',
  styleUrls: ['./account-new.component.scss']
})
export class AccountNewComponent implements OnInit {
  public model: AccountUpsertDto;
  public isLoadingSubmit: boolean;
  public accountID: number;
  accountStatuses: Array<AccountStatusDto>;

  constructor(private accountService: AccountService, private router: Router, private alertService: AlertService, private cdr: ChangeDetectorRef, private accountStatusService: AccountStatusService) { }

  ngOnInit() {
    this.model = new AccountUpsertDto();
    this.accountStatusService.getAccountStatuses().subscribe(accountStatuses => {

        this.accountStatuses = accountStatuses.sort((a: AccountStatusDto, b: AccountStatusDto) => {
          if (a.AccountStatusDisplayName > b.AccountStatusDisplayName)
            return 1;
          if (a.AccountStatusDisplayName < b.AccountStatusDisplayName)
            return -1;
          return 0;
        });
      })
  }

  onSubmit(editAccountForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;

    this.accountService.createAccount(this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        this.router.navigateByUrl("/accounts/" + response.AccountID).then(x => {
          this.alertService.pushAlert(new Alert(`The account was successfully created.`, AlertContext.Success));
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
