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
import { TradeWithMostRecentOfferDto } from 'src/app/shared/models/offer/trade-with-most-recent-offer-dto';
import { WaterTransferDto } from 'src/app/shared/models/water-transfer-dto';
import { PostingService } from 'src/app/services/posting.service';
import { PostingDto } from 'src/app/shared/models/posting/posting-dto';
import { PostingStatusEnum } from 'src/app/shared/models/enums/posting-status-enum';
import { MultiSeriesEntry, WaterUsageDto, MonthlyWaterUsageDto } from 'src/app/shared/models/water-usage-dto';


@Component({
  selector: 'rio-landowner-dashboard',
  templateUrl: './landowner-dashboard.component.html',
  styleUrls: ['./landowner-dashboard.component.scss']
})
export class LandownerDashboardComponent implements OnInit, OnDestroy {
  public waterYearToDisplay: number;
  public currentUser: UserDto;
  private watchUserChangeSubscription: any;
  public showPurchasedDetails: boolean;
  public showSoldDetails: boolean;
  
  public user: UserDto;
  public parcels: Array<ParcelAllocationAndConsumptionDto>;
  public parcelNumbers: string[];
  public postings: Array<PostingDto>;
  public trades: Array<TradeWithMostRecentOfferDto>;
  public waterYears: Array<number>;
  public currentDate: Date;
  public waterTransfers: Array<WaterTransferDto>;
  private tradeStatusIDs: TradeStatusEnum[];
  private postingStatusIDs: PostingStatusEnum[];

  private waterUsage : WaterUsageDto[];
  private waterUsageChartData: {Year: number, ChartData: MultiSeriesEntry[]}[];
  private showMonthlyWaterUseChart: boolean;

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
    this.showMonthlyWaterUseChart = true;
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.waterYears = [2018, 2017, 2016]; //TODO: get this from API
      this.waterYearToDisplay = 2018; //TODO: get this from API
      this.tradeStatusIDs = [TradeStatusEnum.Open];
      this.postingStatusIDs = [PostingStatusEnum.Open];
      this.showPurchasedDetails = false;
      this.showSoldDetails = false;

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
        this.tradeService.getTradeActivityForUser(userID),
        this.userService.getWaterTransfersByUserID(userID),
        this.userService.getWaterUsageByUserID(userID)
      ).subscribe(([parcels, postings, trades, waterTransfers, waterUsage]) => {
        this.parcels = parcels;
        this.parcelNumbers = Array.from(new Set(parcels.map(x=>x.ParcelNumber)));
        
        this.postings = postings;
        this.trades = trades;
        this.waterTransfers = waterTransfers;
        this.waterUsage = waterUsage;
        
        this.waterUsageChartData = waterUsage.map(x=>{
          return {
            Year: x.Year,
            //FIXME: Relying on methods on DTOs is not great. This one just gently reshapes the return value of the API call for the charting library--in future we should actually return the correct shape from the API, or at least shape the data before it comes back from the UserService
            ChartData: x.WaterUsage.map(y=> new MonthlyWaterUsageDto(y).toMultiSeriesEntry())
          }
        });

      });
      this.cdr.detectChanges();
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public toggleTradeStatusShown(): void {
    if (this.tradeStatusIDs.length === 1) {
      this.tradeStatusIDs = [TradeStatusEnum.Accepted, TradeStatusEnum.Open, TradeStatusEnum.Rejected, TradeStatusEnum.Rescinded];
    }
    else {
      this.tradeStatusIDs = [TradeStatusEnum.Open];
    }
  }

  public togglePostingStatusShown(): void {
    if (this.postingStatusIDs.length === 1) {
      this.postingStatusIDs = [PostingStatusEnum.Open, PostingStatusEnum.Closed];
    }
    else {
      this.postingStatusIDs = [PostingStatusEnum.Open];
    }
  }

  public getNoTradesMessage(): string {
    if (this.tradeStatusIDs.length === 1) {
      return "You have no open trades.";
    }
    else {
      return "You have no trade activity.";
    }
  }

  public getNoPostingsMessage(): string {
    if (this.postingStatusIDs.length === 1) {
      return "You have no open postings.";
    }
    else {
      return "You have no posting activity.";
    }
  }

  public getPostingsForWaterYear(): Array<PostingDto> {
    return this.postings
    .filter(x => (new Date(x.PostingDate).getFullYear() - 1).toString() === this.waterYearToDisplay.toString() && this.postingStatusIDs.includes(x.PostingStatus.PostingStatusID))
    .sort((a, b) => a.PostingDate > b.PostingDate ? -1 : a.PostingDate < b.PostingDate ? 1 : 0);
  }

  public getTradesForWaterYear(): Array<TradeWithMostRecentOfferDto> {
    return this.trades
    .filter(x => (new Date(x.OfferDate).getFullYear() - 1).toString() === this.waterYearToDisplay.toString() && this.tradeStatusIDs.includes(x.TradeStatus.TradeStatusID))
    .sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0);
  }

  public getPendingTrades(): Array<TradeWithMostRecentOfferDto> {
    const tradesForWaterYear = this.getTradesForWaterYear();
    const pendingTrades = tradesForWaterYear !== undefined ? this.getTradesForWaterYear().filter(p => p.OfferStatus.OfferStatusID === OfferStatusEnum.Pending) : [];
    return pendingTrades;
  }

  public doesMostRecentOfferBelongToCurrentUser(trade: TradeWithMostRecentOfferDto): boolean {
    return trade.OfferCreateUserID === this.currentUser.UserID;
  }

  public getTradeStatus(trade: TradeWithMostRecentOfferDto): string {
    return (this.doesMostRecentOfferBelongToCurrentUser(trade) ? "You " : "They ")+ trade.TradeStatus.TradeStatusDisplayName.toLowerCase();
  }

  public getTradeDescription(trade: TradeWithMostRecentOfferDto): string {
    return (this.doesMostRecentOfferBelongToCurrentUser(trade) ? trade.OfferPostingTypeID === PostingTypeEnum.OfferToBuy ? "Buying " : "Selling " : trade.OfferPostingTypeID === PostingTypeEnum.OfferToBuy ? "Selling " : "Buying ") + " " + trade.Quantity + " ac-ft";
  }

  public getOfferThatBelongsToYouNotificationText(trade: TradeWithMostRecentOfferDto): string {
    let offerType = trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "sell" : "purchase";
    let respondee = trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "buyer" : "seller";
    return "You have submitted an offer to " + offerType + " " + trade.Quantity + " ac-ft of water. The " + respondee + " has " + this.getDaysLeftToRespond(trade) + " days to respond.";
  }

  public getOfferThatDoesNotBelongToYouNotificationText(trade: TradeWithMostRecentOfferDto): string {
    if(trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell)
    {
      return "A seller is awaiting your response to their offer to sell " + trade.Quantity + " ac-ft of water. This offer expires in " + this.getDaysLeftToRespond(trade) + " days.";
    }
    return "A buyer is awaiting your response to their offer to purchase " + trade.Quantity + " ac-ft of water. This offer expires in " + this.getDaysLeftToRespond(trade) + " days.";
  }

  public getPendingTradePostingType(trade: TradeWithMostRecentOfferDto, isMostRecentOffererTheCurrentUser: boolean): string {
    if(isMostRecentOffererTheCurrentUser)
    {
      return trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "buy" : "sell";
    }
    return trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "sell" : "buy";
  }

  public getRespondeeType(trade: TradeWithMostRecentOfferDto, isMostRecentOffererTheCurrentUser: boolean): string {
    if(isMostRecentOffererTheCurrentUser)
    {
      return trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "buyer" : "seller";
    }
    return trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "seller" : "buyer";
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

  private getWaterTransfersForWaterYear() {
    return this.waterTransfers.filter(x => x.TransferYear - 1 == this.waterYearToDisplay);
  }

  public getSoldWaterTransfersForWaterYear() {
    return this.getWaterTransfersForWaterYear().filter(x => x.TransferringUser.UserID === this.currentUser.UserID);
  }

  public getPurchasedWaterTransfersForWaterYear() {
    return this.getWaterTransfersForWaterYear().filter(x => x.ReceivingUser.UserID === this.currentUser.UserID);
  }

  public isWaterTransferPending(waterTransfer : WaterTransferDto) {
    return !waterTransfer.ConfirmedByReceivingUser || !waterTransfer.ConfirmedByTransferringUser;
  }

  public getPurchasedAcreFeet(): number {
    const waterTransfersForWaterYear = this.getPurchasedWaterTransfersForWaterYear();
    if (waterTransfersForWaterYear.length > 0) {
      return waterTransfersForWaterYear.reduce(function (a, b) {
        return (a + b.AcreFeetTransferred);
      }, 0);
    }
    return null;
  }

  public getSoldAcreFeet(): number {
    const waterTransfersForWaterYear = this.getSoldWaterTransfersForWaterYear();
    if (waterTransfersForWaterYear.length > 0) {
      return waterTransfersForWaterYear.reduce(function (a, b) {
        return (a + b.AcreFeetTransferred);
      }, 0);
    }
    return null;
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

  public getWaterUsageForWaterYear() : MultiSeriesEntry[] {
    return this.waterUsageChartData.find(x=>x.Year == this.waterYearToDisplay).ChartData;
  }

  public toggleChartToDisplay() : void {
    this.showMonthlyWaterUseChart = !this.showMonthlyWaterUseChart;
  }

  public shouldShowMonthlyWaterUseChart() : boolean{
    return this.showMonthlyWaterUseChart ;
  }

  public shouldShowAllocationChart() : boolean{
    return !this.showMonthlyWaterUseChart; 
  }
}
