import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AccountService } from 'src/app/services/account/account.service';

import { forkJoin } from 'rxjs';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { WaterYearService } from 'src/app/services/water-year.service';
import { AccountDto } from 'src/app/shared/generated/model/account-dto';
import { ParcelChangeOwnerDto } from 'src/app/shared/generated/model/parcel-change-owner-dto';
import { ParcelDto } from 'src/app/shared/generated/model/parcel-dto';
import { WaterYearDto } from 'src/app/shared/generated/model/water-year-dto';

@Component({
  selector: 'rio-parcel-change-owner',
  templateUrl: './parcel-change-owner.component.html',
  styleUrls: ['./parcel-change-owner.component.scss']
})
export class ParcelChangeOwnerComponent implements OnInit, OnDestroy {
  public accounts: AccountDto[];
  public selectedAccount: AccountDto;
  public parcelID: number;
  public parcelToBeInactivated: boolean;
  public applyToSubsequentYears: boolean = false;
  public effectiveWaterYear: WaterYearDto;
  public parcel: ParcelDto;

  public isLoadingSubmit: boolean = false;
  public watchAccountChangeSubscription: any;

  public accountDropdownConfig = {
    search: true,
    height: '320px',
    placeholder: "Select an owner from the list of accounts",
    displayKey: "AccountName",
    searchOnKey: "AccountName",
  }

  public waterYearDropdownConfig = {
    search: true,
    height: '320px',
    placeholder: "Select a Water Year from the list of Water Years",
    displayKey: "Year",
    searchOnKey: "Year",
  }

  currentWaterYear: WaterYearDto;
  waterYears: WaterYearDto[];
  loadingFormData: boolean;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private accountService: AccountService,
    private parcelService: ParcelService,
    private authenticationService: AuthenticationService,
    private waterYearService: WaterYearService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.loadingFormData = true;
    this.watchAccountChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.parcelID = parseInt(this.route.snapshot.paramMap.get("id"));
      this.parcelToBeInactivated = false;
      forkJoin(
        this.accountService.listAllAccounts(),
        this.parcelService.getParcelByParcelID(this.parcelID),
        this.waterYearService.getDefaultWaterYearToDisplay(),
        this.waterYearService.getWaterYears()
      ).subscribe(([accounts, parcel, currentWaterYear, waterYears]) => {
        this.accounts = accounts;
        this.parcel = parcel;
        this.currentWaterYear = currentWaterYear;
        this.waterYears = waterYears;
        this.loadingFormData = false;
      });
    });
  }

  ngOnDestroy() {
    this.watchAccountChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public getDisplayName(account: AccountDto): string {
    return account.AccountDisplayName;
  }

  public formValid(): boolean {
    if (!this.parcelToBeInactivated && (this.selectedAccount === null || this.selectedAccount === undefined)) {
      return false;
    }

    if (!this.effectiveWaterYear) {
      return false;
    }

    return true;
  }

  public onSubmit(form: HTMLFormElement) {
    this.isLoadingSubmit = true;
    var parcelChangeOwnerDto = new ParcelChangeOwnerDto({
      ParcelID: this.parcelID,
      AccountID: this.selectedAccount ? this.selectedAccount.AccountID : undefined,
      EffectiveWaterYearID: this.effectiveWaterYear.WaterYearID,
      ApplyToSubsequentYears: this.applyToSubsequentYears
    });
    this.parcelService.changeParcelOwner(this.parcelID, parcelChangeOwnerDto).subscribe(anything => {
      this.isLoadingSubmit = false;
      this.router.navigateByUrl(`/parcels/${this.parcelID}`).then(x => {
        this.alertService.pushAlert(new Alert(`The ownership record for ${this.parcel.ParcelNumber} was successfully updated.`, AlertContext.Success));
      })
    }, error => {
      this.isLoadingSubmit = false;
      this.cdr.detectChanges();
    });
  }

}
