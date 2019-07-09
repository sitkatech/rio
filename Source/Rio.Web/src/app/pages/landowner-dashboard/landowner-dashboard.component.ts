import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { UserService } from 'src/app/services/user/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { ParcelAllocationAndConsumptionDto } from 'src/app/shared/models/parcel/parcel-allocation-and-consumption-dto';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'rio-landowner-dashboard',
  templateUrl: './landowner-dashboard.component.html',
  styleUrls: ['./landowner-dashboard.component.scss']
})
export class LandownerDashboardComponent implements OnInit {

  public user: UserDto;
  public parcels: Array<ParcelAllocationAndConsumptionDto>;
  public currentDate: Date;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private parcelService: ParcelService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef
  ) {
  }

  ngOnInit() {
    const id = parseInt(this.route.snapshot.paramMap.get("id"));
    if (id) {
      this.currentDate = (new Date());
      forkJoin(
        this.userService.getUserFromUserID(id),
        this.parcelService.getParcelAllocationAndConsumptionByUserID(id)
      ).subscribe(([user, parcels]) => {
        this.user = user instanceof Array
          ? null
          : user as UserDto;
        // TODO: scope it to current year for now
        this.parcels = parcels.filter(x => x.WaterYear == 2018);
        this.cdr.detectChanges();
      });
    }
    else {
      forkJoin(
        this.parcelService.getParcelsByUserID(this.authenticationService.currentUser.UserID)
      ).subscribe(([parcels]) => {
        this.user = this.authenticationService.currentUser;
        this.parcels = parcels;
        this.cdr.detectChanges();
      });
    }
  }

  public currentUserIsAdmin(): boolean {
    return this.authenticationService.isAdministrator();
  }

  public getSelectedParcelIDs(): Array<number> {
    return this.parcels !== undefined ? this.parcels.map(p => p.ParcelID) : [];
  }

  public getTotalAPNAcreage(): string {
    if (this.parcels.length > 0) {
      let result = this.parcels.reduce(function (a, b) {
        return (a + b.ParcelAreaInAcres);
      }, 0);
      return result.toFixed(2);
    }
    return "Not available";
  }

  public getAnnualAllocation(): number {
    let parcelsWithAllocation = this.parcels.filter(p => p.AcreFeetAllocated !== null);
    if (parcelsWithAllocation.length > 0) {
      let result = parcelsWithAllocation.reduce(function (a, b) {
        return (a + b.AcreFeetAllocated);
      }, 0);
      return result;
    }
    return null;
  }

  public getLastETReadingDate(): string {
    return "7/1/2019"; //TODO: need to use the date from the latest monthly ET data
  }

  public getWaterConsumption(parcelAllocationAndConsumption: ParcelAllocationAndConsumptionDto): number {
    if (parcelAllocationAndConsumption.MonthlyEvapotranspiration.length > 0) {
      let result = parcelAllocationAndConsumption.MonthlyEvapotranspiration.reduce(function (a, b) {
        return (a + b.EvapotranspirationRate);
      }, 0);
      return result;
    }
    return null;
  }

  public getEstimatedAvailableSupply(): string {
    let annualAllocation = this.getAnnualAllocation();
    let parcelsWithMonthlyEvaporations = this.parcels.filter(x => x.MonthlyEvapotranspiration.length > 0);
    if (annualAllocation !== null || parcelsWithMonthlyEvaporations.length > 0) {
      let estimatedAvailableSupply = this.parcels.reduce((a, b) => {
        return (a + this.getWaterConsumption(b));
      }, 0);
      return (annualAllocation - estimatedAvailableSupply).toFixed(2);
    }
    return "Not available";
  }
}
