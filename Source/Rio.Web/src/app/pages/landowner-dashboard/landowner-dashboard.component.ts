import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { UserService } from 'src/app/services/user/user.service';
import { ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelAllocationAndConsumptionDto } from 'src/app/shared/models/parcel/parcel-allocation-and-consumption-dto';
import { TradeService } from 'src/app/services/trade.service';
import { OfferStatusEnum } from 'src/app/shared/models/enums/offer-status-enum';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';
import { PostingWithTradesWithMostRecentOfferDto } from 'src/app/shared/models/posting/posting-with-trades-with-most-recent-offer-dto';
import { TradeWithMostRecentOfferDto } from 'src/app/shared/models/offer/trade-with-most-recent-offer-dto';
import { WaterYearTransactionDto } from 'src/app/shared/models/water-year-transaction-dto';
import { PostingService } from 'src/app/services/posting.service';
import { PostingDto } from 'src/app/shared/models/posting/posting-dto';
import { PostingStatusEnum } from 'src/app/shared/models/enums/posting-status-enum';

@Component({
  selector: 'rio-landowner-dashboard',
  templateUrl: './landowner-dashboard.component.html',
  styleUrls: ['./landowner-dashboard.component.scss']
})
export class LandownerDashboardComponent implements OnInit, OnDestroy {
  public waterYearToDisplay: number;
  public currentUser: UserDto;
  private watchUserChangeSubscription: any;
  public postingPanels: any;

  public user: UserDto;
  public parcels: Array<ParcelAllocationAndConsumptionDto>;
  public postings: Array<PostingDto>;
  public trades: Array<TradeWithMostRecentOfferDto>;
  public waterYears: Array<number>;
  public currentDate: Date;
  public waterYearTransactions: Array<WaterYearTransactionDto>;
  private tradeStatusIDs: TradeStatusEnum[];
  private postingStatusIDs: PostingStatusEnum[];

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private postingService: PostingService,
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
      this.tradeStatusIDs = [TradeStatusEnum.Open];
      this.postingStatusIDs = [PostingStatusEnum.Open];

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
        this.postingService.getPostingsByUserID(userID),
        this.tradeService.getActiveTradesForUser(userID),
        this.userService.getWaterYearAcresTransactedForUser(userID)
      ).subscribe(([parcels, postings, trades, waterYearTransactions]) => {
        this.parcels = parcels;
        this.postings = postings;
        this.trades = trades;
        this.waterYearTransactions = waterYearTransactions;
        this.postingPanels = postings.reduce((map, obj) => {
          map[obj.PostingID] = false;
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

  public hideShowPanel(postingID: number): boolean {
    return this.postingPanels[postingID];
  }

  public toggleTradeStatusShown(): void {
    if(this.tradeStatusIDs.length === 1)
    {
      this.tradeStatusIDs = [TradeStatusEnum.Accepted, TradeStatusEnum.Open, TradeStatusEnum.Rejected, TradeStatusEnum.Rescinded];
    }
    else
    {
      this.tradeStatusIDs = [TradeStatusEnum.Open];
    }
  }

  public togglePostingStatusShown(): void {
    if(this.postingStatusIDs.length === 1)
    {
      this.postingStatusIDs = [PostingStatusEnum.Open, PostingStatusEnum.Closed];
    }
    else
    {
      this.postingStatusIDs = [PostingStatusEnum.Open];
    }
  }

  public currentUserIsAdmin(): boolean {
    return this.authenticationService.isUserAnAdministrator(this.currentUser);
  }

  public getPostingsForWaterYear(): Array<PostingDto> {
    return this.postings.filter(x => (new Date(x.PostingDate).getFullYear() - 1).toString() === this.waterYearToDisplay.toString() && this.postingStatusIDs.includes(x.PostingStatus.PostingStatusID));
  }

  public getTradesForWaterYear(): Array<TradeWithMostRecentOfferDto> {
    return this.trades.filter(x => (new Date(x.OfferDate).getFullYear() - 1).toString() === this.waterYearToDisplay.toString() && this.tradeStatusIDs.includes(x.TradeStatus.TradeStatusID));
  }

  public getPendingTrades(): Array<TradeWithMostRecentOfferDto> {
    const tradesForWaterYear = this.getTradesForWaterYear();
    const pendingTrades = tradesForWaterYear !== undefined ? this.getTradesForWaterYear().filter(p => p.OfferStatus.OfferStatusID === OfferStatusEnum.Pending) : [];
    return pendingTrades;
  }

  public doesMostRecentOfferBelongToCurrentUser(trade: TradeWithMostRecentOfferDto): boolean {
    return trade.OfferCreateUserID === this.currentUser.UserID;
  }

  public getTradeDescription(trade: TradeWithMostRecentOfferDto): string {
    return (this.doesMostRecentOfferBelongToCurrentUser(trade) ? trade.OfferPostingTypeID === PostingTypeEnum.OfferToBuy ? "Buying " : "Selling " : trade.OfferPostingTypeID === PostingTypeEnum.OfferToBuy ? "Selling " : "Buying ") + " " + trade.Quantity + " ac-ft";
  }

  public getPendingTradePostingType(trade: TradeWithMostRecentOfferDto, isMostRecentOffererTheCurrentUser: boolean): string {
    if (trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell) {
      return !isMostRecentOffererTheCurrentUser ? "purchase" : "sell";
    }
    return isMostRecentOffererTheCurrentUser ? "purchase" : "sell";
  }

  public getRespondeeType(trade: TradeWithMostRecentOfferDto, isMostRecentOffererTheCurrentUser: boolean): string {
    if (trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell) {
      return !isMostRecentOffererTheCurrentUser ? "seller" : "buyer";
    }
    return isMostRecentOffererTheCurrentUser ? "seller" : "buyer";
  }

  public getDaysLeftToRespond(trade: TradeWithMostRecentOfferDto): number {
    //TODO: get logic to calculated days left to respond; hardcoded to 5 for now
    return 5;
  }

  public getParcelsForWaterYear(): Array<ParcelAllocationAndConsumptionDto> {
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
    const waterYearTransaction = this.waterYearTransactions.find(x => x.WaterYear - 1 == this.waterYearToDisplay);
    if (waterYearTransaction) {
      if (postingTypeEnum === PostingTypeEnum.OfferToSell) {
        return waterYearTransaction.AcreFeetSold;
      }
      return waterYearTransaction.AcreFeetPurchased;
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

  public getTradesForPosting(posting: PostingWithTradesWithMostRecentOfferDto): Array<TradeWithMostRecentOfferDto> {    
    return posting.Trades.sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0);
  }
}
