import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { UserService } from 'src/app/services/user/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelAllocationAndConsumptionDto } from 'src/app/shared/models/parcel/parcel-allocation-and-consumption-dto';
import { TradeService } from 'src/app/services/trade.service';
import { TradeWithMostRecentOfferDto } from 'src/app/shared/models/offer/trade-with-most-recent-offer-dto';
import { OfferStatusDto } from 'src/app/shared/models/offer/offer-status-dto';
import { OfferStatusEnum } from 'src/app/shared/models/enums/offer-status-enum';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';

@Component({
  selector: 'rio-landowner-dashboard',
  templateUrl: './landowner-dashboard.component.html',
  styleUrls: ['./landowner-dashboard.component.scss']
})
export class LandownerDashboardComponent implements OnInit, OnDestroy {
  public currentUser: UserDto;
  private watchUserChangeSubscription: any;

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
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;

      this.currentDate = (new Date());
      let userID = parseInt(this.route.snapshot.paramMap.get("id"));
      if (userID) {
        this.userService.getUserFromUserID(userID).subscribe(user => {
          this.user = user instanceof Array
            ? null
            : user as UserDto;
        });
      }
      else {
        userID = this.currentUser.UserID;
        this.user = this.currentUser;
      }
      forkJoin(
        this.parcelService.getParcelAllocationAndConsumptionByUserID(userID),
        this.tradeService.getActiveTradesForUser(userID)
      ).subscribe(([parcels, trades]) => {
        // TODO: scope it to current year for now
        this.parcels = parcels.filter(x => x.WaterYear == 2018);
        this.trades = trades;
        this.cdr.detectChanges();
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public currentUserIsAdmin(): boolean {
    return this.authenticationService.isUserAnAdministrator(this.currentUser);
  }

  public getPendingTrades(): Array<TradeWithMostRecentOfferDto> {
    return this.trades !== undefined ? this.trades.filter(p => p.OfferStatus.OfferStatusID === OfferStatusEnum.Pending) : [];
  }

  public doesMostRecentOfferBelongToCurrentUser(trade: TradeWithMostRecentOfferDto): boolean {
    return trade.OfferCreateUserID === this.currentUser.UserID;
  }

  public getPendingTradePostingType(trade: TradeWithMostRecentOfferDto): string {
    const originalPostingUserIsCurrentUser = trade.Posting.CreateUser.UserID === this.currentUser.UserID;
    const originalPostingTypeIsOfferToSell = trade.Posting.PostingType.PostingTypeID === PostingTypeEnum.OfferToSell;
    if (originalPostingUserIsCurrentUser) {
      return originalPostingTypeIsOfferToSell ? "sell" : "purchase";
    }
    return originalPostingTypeIsOfferToSell ? "purchase" : "sell";
  }

  public getRespondeeType(trade: TradeWithMostRecentOfferDto, isMostRecentOffererTheCurrentUser: boolean): string {
    const originalPostingUserIsCurrentUser = trade.Posting.CreateUser.UserID === this.currentUser.UserID;
    const originalPostingTypeIsOfferToSell = trade.Posting.PostingType.PostingTypeID === PostingTypeEnum.OfferToSell;
    if (isMostRecentOffererTheCurrentUser) {
      if (originalPostingUserIsCurrentUser) {
        return originalPostingTypeIsOfferToSell ? "buyer" : "seller";
      }
      return originalPostingTypeIsOfferToSell ? "seller" : "buyer";
    }
    else {
      if (originalPostingUserIsCurrentUser) {
        return originalPostingTypeIsOfferToSell ? "buyer" : "seller";
      }
      return originalPostingTypeIsOfferToSell ? "seller" : "buyer";
    }
  }

  public getDaysLeftToRespond(trade: TradeWithMostRecentOfferDto): number {
    //TODO: get logic to calculated days left to respond; hardcoded to 5 for now
    return 5;
  }

  public getSelectedParcelIDs(): Array<number> {
    return this.parcels !== undefined ? this.parcels.map(p => p.ParcelID) : [];
  }

  public getTotalAPNAcreage(): string {
    if (this.parcels.length > 0) {
      let result = this.parcels.reduce(function (a, b) {
        return (a + b.ParcelAreaInAcres);
      }, 0);
      return result.toFixed(1) + " ac";
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
    return "12/31/2018"; //TODO: need to use the date from the latest monthly ET data
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
