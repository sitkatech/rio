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
import { OfferStatusEnum } from 'src/app/shared/models/enums/offer-status-enum';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';

@Component({
  selector: 'rio-landowner-dashboard',
  templateUrl: './landowner-dashboard.component.html',
  styleUrls: ['./landowner-dashboard.component.scss']
})
export class LandownerDashboardComponent implements OnInit, OnDestroy {
  public waterYearToDisplay: number;
  public currentUser: UserDto;
  private watchUserChangeSubscription: any;
  public postingPanels : any;

  public user: UserDto;
  public parcels: Array<ParcelAllocationAndConsumptionDto>;
  public trades: Array<TradeWithMostRecentOfferDto>;
  public waterYears: Array<number>;
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
      this.waterYears = [2018, 2017, 2016]; //TODO: get this from API
      this.waterYearToDisplay = 2018; //TODO: get this from API

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
        this.parcels = parcels;
        this.trades = trades;
        this.postingPanels = Array.from(new Set(this.trades.map((item: TradeWithMostRecentOfferDto) => item.Posting.PostingID))).reduce((map, obj) => {
          map[obj] = false;
          return map;
        }, {});
      });
      this.cdr.detectChanges();
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public hideShowPanel(postingID: number) : boolean{
    return this.postingPanels[postingID];
  }

  public currentUserIsAdmin(): boolean {
    return this.authenticationService.isUserAnAdministrator(this.currentUser);
  }

  public getTradesForWaterYear(): Array<TradeWithMostRecentOfferDto> {
    return this.trades.filter(x => (new Date(x.Posting.PostingDate).getFullYear() - 1).toString() === this.waterYearToDisplay.toString());
  }

  public getTradesGroupedByPostingForWaterYear(): any {
    return this.getTradesForWaterYear().reduce(function (h, obj) {
      h[obj.Posting.PostingID] = (h[obj.Posting.PostingID] || []).concat(obj);
      return h;
    }, {});
  }

  public getPendingTrades(): Array<TradeWithMostRecentOfferDto> {
    const tradesForWaterYear = this.getTradesForWaterYear();
    return tradesForWaterYear !== undefined ? this.getTradesForWaterYear().filter(p => p.OfferStatus.OfferStatusID === OfferStatusEnum.Pending) : [];
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

  public getParcelsForWaterYear() : Array<ParcelAllocationAndConsumptionDto>
  {
    return this.parcels.filter(x => x.WaterYear.toString() === this.waterYearToDisplay.toString());
  }

  public getSelectedParcelIDs(): Array<number> {
    const parcelsForWaterYear = this.getParcelsForWaterYear();
    return parcelsForWaterYear !== undefined ? this.getParcelsForWaterYear().map(p => p.ParcelID) : [];
  }

  public getTotalAPNAcreage(): string {
    const parcelsForWaterYear = this.getParcelsForWaterYear();
    if (parcelsForWaterYear.length > 0) {
      let result = parcelsForWaterYear.reduce(function (a, b) {
        return (a + b.ParcelAreaInAcres);
      }, 0);
      return result.toFixed(1) + " ac";
    }
    return "Not available";
  }

  public getAnnualAllocation(): number {
    let parcelsWithAllocation = this.getParcelsForWaterYear().filter(p => p.AcreFeetAllocated !== null);
    if (parcelsWithAllocation.length > 0) {
      let result = parcelsWithAllocation.reduce(function (a, b) {
        return (a + b.AcreFeetAllocated);
      }, 0);
      return result;
    }
    return null;
  }

  public getLastETReadingDate(): string {
    return "12/31/" + this.waterYearToDisplay; //TODO: need to use the date from the latest monthly ET data
  }

  private getTotalAcreFeetBoughtOrSold(postingTypeEnum: PostingTypeEnum): number {
    const acceptedPurchases = this.getTradesForWaterYear().filter(x => x.TradeStatus.TradeStatusID === TradeStatusEnum.Accepted
      && ((this.currentUser.UserID == x.Posting.CreateUser.UserID && x.Posting.PostingType.PostingTypeID === postingTypeEnum)
        || (this.currentUser.UserID != x.Posting.CreateUser.UserID && x.Posting.PostingType.PostingTypeID !== postingTypeEnum)
      )    
    );
    if (acceptedPurchases.length > 0) {
      let result = acceptedPurchases.reduce(function (a, b) {
        return (a + b.Quantity);
      }, 0);
      return result;
    }
    return null;
  }

  public getPurchasedAcreFeet(): number {
    return this.getTotalAcreFeetBoughtOrSold(PostingTypeEnum.OfferToBuy);
  }

  public getSoldAcreFeet(): number {
    return this.getTotalAcreFeetBoughtOrSold(PostingTypeEnum.OfferToSell);
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

  public getWaterUsageToDate(): number {
    let parcelsWithMonthlyEvaporations = this.getParcelsForWaterYear().filter(x => x.MonthlyEvapotranspiration.length > 0);
    if (parcelsWithMonthlyEvaporations.length > 0) {
      let estimatedAvailableSupply = parcelsWithMonthlyEvaporations.reduce((a, b) => {
        return (a + this.getWaterConsumption(b));
      }, 0);
      return estimatedAvailableSupply;
    }
    return null;
  }

  public getEstimatedAvailableSupply(): number {
    let annualAllocation = this.getAnnualAllocation();
    let estimatedAvailableSupply = this.getWaterUsageToDate();
    if (annualAllocation !== null && estimatedAvailableSupply !== null) {
      return annualAllocation - estimatedAvailableSupply;
    }
    return null;
  }
}
