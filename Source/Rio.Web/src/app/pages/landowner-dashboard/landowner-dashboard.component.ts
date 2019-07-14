import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { UserService } from 'src/app/services/user/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelAllocationAndConsumptionDto } from 'src/app/shared/models/parcel/parcel-allocation-and-consumption-dto';
import { TradeService } from 'src/app/services/trade.service';
import { TradeWithMostRecentOfferDto } from 'src/app/shared/models/offer/trade-with-most-recent-offer-dto';

@Component({
  selector: 'rio-landowner-dashboard',
  templateUrl: './landowner-dashboard.component.html',
  styleUrls: ['./landowner-dashboard.component.scss']
})
export class LandownerDashboardComponent implements OnInit {

  public user: UserDto;
  public parcels: Array<ParcelAllocationAndConsumptionDto>;
  public trades: Array<TradeWithMostRecentOfferDto>;
  public currentDate: Date;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private parcelService: ParcelService,
    private tradeService: TradeService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef
  ) {
  }

  ngOnInit() {
    let userID = parseInt(this.route.snapshot.paramMap.get("id"));
    if (userID) {
      this.currentDate = (new Date());
      forkJoin(
        this.userService.getUserFromUserID(userID),
        this.parcelService.getParcelAllocationAndConsumptionByUserID(userID),
        this.tradeService.getActiveTradesForUser(userID)
      ).subscribe(([user, parcels, trades]) => {
        this.user = user instanceof Array
          ? null
          : user as UserDto;
        // TODO: scope it to current year for now
        this.parcels = parcels.filter(x => x.WaterYear == 2018);
        this.trades = trades;
        this.cdr.detectChanges();
      });
    }
    else {
      userID = this.authenticationService.currentUser.UserID;
      forkJoin(
        this.parcelService.getParcelsByUserID(userID),
        this.tradeService.getActiveTradesForUser(userID)
      ).subscribe(([parcels, trades]) => {
        this.user = this.authenticationService.currentUser;
        this.parcels = parcels;
        this.trades = trades;
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
      return result.toFixed(1);
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
      return (annualAllocation - estimatedAvailableSupply).toFixed(1);
    }
    return "Not available";
  }
}
