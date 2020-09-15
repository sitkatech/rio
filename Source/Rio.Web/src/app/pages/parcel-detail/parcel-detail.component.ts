import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { isNullOrUndefined } from 'util';
import { UserDto } from 'src/app/shared/models';
import { ParcelAllocationTypeEnum } from 'src/app/shared/models/enums/parcel-allocation-type-enum';
import { ParcelAllocationDto } from 'src/app/shared/models/parcel/parcel-allocation-dto';
import { ParcelMonthlyEvapotranspirationDto } from 'src/app/shared/models/parcel/parcel-monthly-evapotranspiration-dto';
import { ParcelOwnershipDto } from 'src/app/shared/models/parcel/parcel-ownership-dto';
import { ParcelAllocationTypeService } from 'src/app/services/parcel-allocation-type.service';
import { ParcelAllocationTypeDto } from 'src/app/shared/models/parcel-allocation-type-dto';

@Component({
  selector: 'template-parcel-detail',
  templateUrl: './parcel-detail.component.html',
  styleUrls: ['./parcel-detail.component.scss']
})
export class ParcelDetailComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public waterYears: Array<number>;
  public parcel: ParcelDto;
  public parcelAllocations: Array<ParcelAllocationDto>;
  public waterUsage: Array<ParcelMonthlyEvapotranspirationDto>;
  public months: number[];
  public parcelOwnershipHistory: ParcelOwnershipDto[];

  public today: Date = new Date();
  public parcelAllocationTypes: ParcelAllocationTypeDto[];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private parcelService: ParcelService,
    private authenticationService: AuthenticationService,
    private parcelAllocationTypeService: ParcelAllocationTypeService,
    private cdr: ChangeDetectorRef
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;

      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      if (id) {
        forkJoin(
          this.parcelService.getParcelByParcelID(id),
          this.parcelService.getParcelAllocations(id),
          this.parcelService.getWaterUsage(id),
          this.parcelService.getParcelOwnershipHistory(id),
          this.parcelService.getWaterYears(),
          this.parcelAllocationTypeService.getParcelAllocationTypes()
        ).subscribe(([parcel, parcelAllocations, waterUsage, parcelOwnershipHistory, waterYears, parcelAllocationTypes]) => {
          this.parcel = parcel instanceof Array
            ? null
            : parcel as ParcelDto;
          this.parcelAllocations = parcelAllocations;
          console.log(parcelAllocations);
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

  // public getProjectWaterForYear(year: number): string {
  //   return this.getAllocationForYearByType(ParcelAllocationTypeEnum.ProjectWater, year);
  // }

  // public getReconciliationForYear(year: number): string {
  //   return this.getAllocationForYearByType(ParcelAllocationTypeEnum.Reconciliation, year);
  // }

  // public getNativeYieldForYear(year: number): string {
  //   return this.getAllocationForYearByType(ParcelAllocationTypeEnum.NativeYield, year);
  // }

  // public getStoredWaterForYear(year: number): string {
  //   return this.getAllocationForYearByType(ParcelAllocationTypeEnum.StoredWater, year);
  // }

  public getTotalAllocationForYear(year: number): string {
    var parcelAllocationsForYear = this.parcelAllocations.filter(x => x.WaterYear === year);
    if (parcelAllocationsForYear.length > 0) {
      let result = parcelAllocationsForYear.reduce(function (a, b) {
        return (a + b.AcreFeetAllocated);
      }, 0);
      return result.toFixed(1);
    }
    else {
      return "-";
    }
  }

  private getAllocationForYearByType(parcelAllocationType: ParcelAllocationTypeDto, year: number): string {
    var parcelAllocation = this.parcelAllocations.find(x => x.WaterYear === year && x.ParcelAllocationTypeID === parcelAllocationType.ParcelAllocationTypeID);
    return parcelAllocation ? parcelAllocation.AcreFeetAllocated.toFixed(1) : "-";
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
    return this.parcelOwnershipHistory.filter(x=>{
      return x.EffectiveYear <= this.today.getFullYear();      
    })[0]
  }
  
  public isAdministrator() : boolean
  {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }
}
