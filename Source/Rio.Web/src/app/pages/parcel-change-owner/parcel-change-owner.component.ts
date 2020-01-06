import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AccountService } from 'src/app/services/account/account.service';

import { forkJoin } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { IMyDpOptions } from 'mydatepicker';
import { ParcelChangeOwnerDto } from 'src/app/shared/models/parcel/parcel-change-owner-dto';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { AccountDto } from 'src/app/shared/models/account/account-dto';
import { MdpDate } from '../../shared/models/mdp-date';

@Component({
  selector: 'rio-parcel-change-owner',
  templateUrl: './parcel-change-owner.component.html',
  styleUrls: ['./parcel-change-owner.component.scss']
})
export class ParcelChangeOwnerComponent implements OnInit, OnDestroy {
  public accounts: AccountDto[];
  public selectedAccount: AccountDto;
  public parcelID: number;
  public ownerHasAccount: boolean;
  public effectiveYear: number;
  public saleDate: MdpDate;
  public parcel: ParcelDto;
  public ownerNameUntracked: string;
  public note: string = "";

  public isLoadingSubmit: boolean = false;
  public watchAccountChangeSubscription: any;

  public myDatePickerOptions: IMyDpOptions = {
    // other options...
    dateFormat: 'mm-dd-yyyy',
  };

  public accountDropdownConfig = {
    search: true,
    height: '320px',
    placeholder: "Select an owner from the list of accounts",
    displayKey: "AccountName",
    searchOnKey: "AccountName",
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private accountService: AccountService,
    private parcelService: ParcelService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    const today = new Date();
    this.saleDate = {date: {year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate()}, jsdate: today};
    console.log(this.saleDate)
    this.watchAccountChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.parcelID = parseInt(this.route.snapshot.paramMap.get("id"));
      this.ownerHasAccount = true;
      forkJoin(
        this.accountService.listAllAccounts(),
        this.parcelService.getParcelByParcelID(this.parcelID)
      ).subscribe(([accounts, parcel]) => {
        this.accounts = accounts;
        this.parcel = parcel;
      });
    });
  }

  ngOnDestroy() {
    this.watchAccountChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public getDisplayName(account: AccountDto): string {
    return `${account.AccountNumber} - ${account.AccountName}`;
  }

  public formValid(): boolean {
    const lengthValid  = this.note.length <= 500 ;
    let accountValid;
    if (this.ownerHasAccount){
      accountValid = (Boolean) ((this.selectedAccount && this.selectedAccount.AccountID));
    }
    else{
      accountValid = this.ownerNameUntracked;
    }
     
    const valid = lengthValid && accountValid;
    return valid;
  }

  public onSubmit(form: HTMLFormElement) {
    debugger;
    var associativeArray = {
      ParcelID: this.parcelID,
      AccountID: this.selectedAccount ? this.selectedAccount.AccountID : undefined,
      OwnerName: this.ownerNameUntracked,
      SaleDate: this.saleDate.jsdate,
      EffectiveYear: this.effectiveYear,
      Note: this.note

    }
    this.isLoadingSubmit = true;
    var parcelChangeOwnerDto = new ParcelChangeOwnerDto(associativeArray);
    this.parcelService.changeParcelOwner(this.parcelID, parcelChangeOwnerDto).subscribe(anything => {
      this.isLoadingSubmit = false;
      form.reset();
      this.router.navigateByUrl(`/parcels/${this.parcelID}`).then(x => {
        this.alertService.pushAlert(new Alert(`The ownership record for ${this.parcel.ParcelNumber} was successfully updated.`, AlertContext.Success));
      })
    }, error => {
      this.isLoadingSubmit = false;
      this.cdr.detectChanges();
    });
  }

}
