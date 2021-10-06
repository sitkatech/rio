import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { isNullOrUndefined } from 'util';
import { UserDto } from 'src/app/shared/models';
import { ParcelLedgerDto } from 'src/app/shared/models/parcel/parcel-ledger-dto';
import { ParcelMonthlyEvapotranspirationDto } from 'src/app/shared/models/parcel/parcel-monthly-evapotranspiration-dto';
import { ParcelOwnershipDto } from 'src/app/shared/models/parcel/parcel-ownership-dto';
import { AccountSimpleDto } from 'src/app/shared/models/account/account-simple-dto';
import { AccountService } from 'src/app/services/account/account.service';
import { WaterYearDto } from "src/app/shared/models/water-year-dto";
import { WaterYearService } from 'src/app/services/water-year.service';
import { ParcelAllocationTypeDto } from 'src/app/shared/models/parcel-allocation-type-dto';
import { ParcelAllocationTypeService } from 'src/app/services/parcel-allocation-type.service';

@Component({
  selector: 'template-parcel-detail',
  templateUrl: './parcel-detail.component.html',
  styleUrls: ['./parcel-detail.component.scss']
})
export class ParcelDetailComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  private currentUserAccounts: AccountSimpleDto[];

  public waterYears: Array<WaterYearDto>;
  public parcel: ParcelDto;
  public parcelLedgers: Array<ParcelLedgerDto>;
  public waterUsage: Array<ParcelMonthlyEvapotranspirationDto>;
  public months: number[];
  public parcelOwnershipHistory: ParcelOwnershipDto[];

  public today: Date = new Date();
  public parcelAllocationTypes: ParcelAllocationTypeDto[];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
    private authenticationService: AuthenticationService,
    private parcelAllocationTypeService: ParcelAllocationTypeService,
    private accountService: AccountService,
    private cdr: ChangeDetectorRef
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.currentUserAccounts = this.authenticationService.getAvailableAccounts();
      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      if (id) {
        forkJoin(
          this.parcelService.getParcelByParcelID(id),
          this.parcelService.getParcelAllocations(id),
          this.parcelService.getWaterUsage(id),
          this.parcelService.getParcelOwnershipHistory(id),
          this.waterYearService.getWaterYears(),
          this.parcelAllocationTypeService.getParcelAllocationTypes()
        ).subscribe(([parcel, parcelLedgers, waterUsage, parcelOwnershipHistory, waterYears, parcelAllocationTypes]) => {
          this.parcel = parcel instanceof Array
            ? null
            : parcel as ParcelDto;
          this.parcelLedgers = parcelLedgers;
          this.waterUsage = waterUsage;
          this.waterYears = waterYears;
          this.parcelOwnershipHistory = parcelOwnershipHistory;
          this.parcelAllocationTypes = parcelAllocationTypes;
        });
      }
      this.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public getSelectedParcelIDs(): Array<number> {
    return this.parcel !== undefined ? [this.parcel.ParcelID] : [];
  }

  public getConsumptionForYearAndMonth(year: number, month: number): string {
    var monthlyEvapotranspiration = this.waterUsage.find(x => x.WaterYear === year && x.WaterMonth === month);
    return isNullOrUndefined(monthlyEvapotranspiration) ? "-" : isNullOrUndefined(monthlyEvapotranspiration.OverriddenEvapotranspirationRate) ? monthlyEvapotranspiration.EvapotranspirationRate.toFixed(1) : monthlyEvapotranspiration.OverriddenEvapotranspirationRate.toFixed(1);
  }
  
  public getTotalAllocationForYear(year: number): string {
    var parcelLedgerForYear = this.parcelLedgers.filter(x => x.WaterYear === year);
    if (parcelLedgerForYear.length > 0) {
      let result = parcelLedgerForYear.reduce(function (a, b) {
        return (a + b.TransactionAmount);
      }, 0);
      return result.toFixed(1);
    }
    else {
      return "-";
    }
  }

  public getAllocationForYearByType(parcelAllocationType: ParcelAllocationTypeDto, year: number): string {
    var parcelLedger = this.parcelLedgers.find(x => x.WaterYear === year && x.WaterTypeID === parcelAllocationType.ParcelAllocationTypeID);
    return parcelLedger ? parcelLedger.TransactionAmount.toFixed(1) : "-";
  }

  public getConsumptionForYear(year: number): string {
    var monthlyEvapotranspiration = this.waterUsage.filter(x => x.WaterYear === year);
    if (monthlyEvapotranspiration.length > 0) {
      let result = monthlyEvapotranspiration.reduce(function (a, b) {
        return (a + (b.OverriddenEvapotranspirationRate || b.EvapotranspirationRate));
      }, 0);
      return result.toFixed(1);
    }
    else {
      return "-";
    }
  }

  public getCurrentOwner(): ParcelOwnershipDto{  
    
    var currentOwner = this.parcelOwnershipHistory.filter(x=>{
      return x.WaterYear.Year == this.today.getFullYear();      
    });

    return currentOwner.length == 1 ? currentOwner[0] : null;
  }

  public getCurrentOwnerAccountNumber(): number {
    let currentOwner = this.getCurrentOwner();
    if (!currentOwner || !currentOwner.Account.AccountID || !this.currentUserAccounts || this.currentUserAccounts.length == 0) {
      return null;
    }
    
    let account = this.currentUserAccounts.filter(x => x.AccountID == currentOwner.Account.AccountID)[0];

    if (!account) {
      return null;
    }
    
    return account.AccountNumber
  }

  public isActive() : boolean {
    if (!this.parcel) {
      return false;
    }

    let currentOwner = this.getCurrentOwner();

    return currentOwner != null && currentOwner != undefined;
  }
  
  public isAdministrator() : boolean
  {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }

  public isDemoUser() : boolean
  {
    return this.authenticationService.isCurrentUserADemoUser();
  }

  public isLandowner(): boolean
  {
    return this.authenticationService.isCurrentUserALandOwner();
  }

  public showGoToDashboardButton(): boolean {
    let currentOwner = this.getCurrentOwner();
    return this.currentUser && this.currentUserAccounts && this.currentUserAccounts.length > 0 && currentOwner && currentOwner.Account && this.currentUserAccounts.some(x => x.AccountID == currentOwner.Account.AccountID);
  }
}
