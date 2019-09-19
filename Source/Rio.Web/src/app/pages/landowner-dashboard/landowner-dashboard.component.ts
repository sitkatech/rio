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
import { WaterAllocationOverviewDto, WaterUsageDto } from 'src/app/shared/models/water-usage-dto';
import { MultiSeriesEntry, SeriesEntry } from "src/app/shared/models/series-entry";


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

  private waterUsageChartData: { Year: number, ChartData: MultiSeriesEntry[] }[];
  private waterUsageOverview: WaterAllocationOverviewDto;
  private historicCumulativeWaterUsage: MultiSeriesEntry;
  private annualAllocationChartData: { Year: number, ChartData: MultiSeriesEntry }[];
  private allocationChartRange: number[];
  historicAverageAnnualUsage: string | number;

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
      this.tradeStatusIDs = [TradeStatusEnum.Countered];
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
        this.userService.getWaterUsageByUserID(userID),
        this.userService.getWaterUsageOverviewByUserID(userID)
      ).subscribe(([parcels, postings, trades, waterTransfers, waterUsage, waterUsageOverview]) => {
        this.parcels = parcels;
        this.parcelNumbers = Array.from(new Set(parcels.map(x => x.ParcelNumber)));

        this.postings = postings;
        this.trades = trades;
        this.waterTransfers = waterTransfers;

        this.initializeCharts(waterUsage, waterUsageOverview);
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
      this.tradeStatusIDs = [TradeStatusEnum.Accepted, TradeStatusEnum.Countered, TradeStatusEnum.Rejected, TradeStatusEnum.Rescinded];
    }
    else {
      this.tradeStatusIDs = [TradeStatusEnum.Countered];
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
    return this.trades ?
      this.trades
        .filter(x => (new Date(x.OfferDate).getFullYear() - 1).toString() === this.waterYearToDisplay.toString() && (this.tradeStatusIDs.includes(x.TradeStatus.TradeStatusID) || (x.OfferStatus.OfferStatusID === OfferStatusEnum.Accepted && (!x.IsConfirmedByBuyer || !x.IsConfirmedBySeller))))
        .sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0) : [];
  }

  public getPendingTrades(): Array<TradeWithMostRecentOfferDto> {
    const tradesForWaterYear = this.getTradesForWaterYear();
    const pendingTrades = tradesForWaterYear !== undefined ? this.getTradesForWaterYear().filter(p => p.OfferStatus.OfferStatusID === OfferStatusEnum.Pending) : [];
    return pendingTrades;
  }

  public doesMostRecentOfferBelongToCurrentUser(trade: TradeWithMostRecentOfferDto): boolean {
    return trade.OfferCreateUser.UserID === this.user.UserID;
  }

  public getTradeStatus(trade: TradeWithMostRecentOfferDto): string {
    return (this.doesMostRecentOfferBelongToCurrentUser(trade) ? "You " : "They ") + trade.TradeStatus.TradeStatusDisplayName.toLowerCase();
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
    if (trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell) {
      return "A seller is awaiting your response to their offer to sell " + trade.Quantity + " ac-ft of water. This offer expires in " + this.getDaysLeftToRespond(trade) + " days.";
    }
    return "A buyer is awaiting your response to their offer to purchase " + trade.Quantity + " ac-ft of water. This offer expires in " + this.getDaysLeftToRespond(trade) + " days.";
  }

  public getPendingTradePostingType(trade: TradeWithMostRecentOfferDto, isMostRecentOffererTheCurrentUser: boolean): string {
    if (isMostRecentOffererTheCurrentUser) {
      return trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "buy" : "sell";
    }
    return trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "sell" : "buy";
  }

  public getRespondeeType(trade: TradeWithMostRecentOfferDto, isMostRecentOffererTheCurrentUser: boolean): string {
    if (isMostRecentOffererTheCurrentUser) {
      return trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "buyer" : "seller";
    }
    return trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "seller" : "buyer";
  }

  public isTradeConfirmedByUser(trade: TradeWithMostRecentOfferDto) {
    return (trade.IsConfirmedByBuyer && trade.Buyer.UserID === this.user.UserID) || (trade.IsConfirmedBySeller && trade.Seller.UserID === this.user.UserID);
  }

  public isTradeConfirmedByBothParties(trade: TradeWithMostRecentOfferDto) {
    return trade.IsConfirmedByBuyer && trade.IsConfirmedBySeller;
  }

  public getDaysLeftToRespond(trade: TradeWithMostRecentOfferDto): number {
    //TODO: get logic to calculated days left to respond; hardcoded to 5 for now
    return 5;
  }

  public getParcelsForWaterYear(): Array<ParcelAllocationAndConsumptionDto> {
    return this.parcels.filter(x => x.WaterYear.toString() === this.waterYearToDisplay.toString());
  }

  public getParcelsForSpecificWaterYear(year: number) {
    return this.parcels.filter(x => x.WaterYear.toString() === year.toString());
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

  public getAnnualAllocationForSpecificWaterYear(year: number): number {
    let parcelsWithAllocation = this.getParcelsForSpecificWaterYear(year).filter(p => p.AcreFeetAllocated !== null);
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

  private getWaterTransfersForWaterYear(year?: number) {
    if (!year) {
      year = this.waterYearToDisplay
    }
    return this.waterTransfers.filter(x => x.TransferYear - 1 == year);
  }

  public getSoldWaterTransfersForWaterYear(year?: number) {
    return this.getWaterTransfersForWaterYear(year).filter(x => x.TransferringUser.UserID === this.user.UserID);
  }

  public getPurchasedWaterTransfersForWaterYear(year?: number) {
    return this.getWaterTransfersForWaterYear(year).filter(x => x.ReceivingUser.UserID === this.user.UserID);
  }

  public isWaterTransferPending(waterTransfer: WaterTransferDto) {
    return !waterTransfer.ConfirmedByReceivingUser || !waterTransfer.ConfirmedByTransferringUser;
  }

  public getPurchasedAcreFeet(year?: number): number {
    const waterTransfersForWaterYear = this.getPurchasedWaterTransfersForWaterYear(year);
    if (waterTransfersForWaterYear.length > 0) {
      return waterTransfersForWaterYear.reduce(function (a, b) {
        return (a + b.AcreFeetTransferred);
      }, 0);
    }
    return null;
  }

  public getSoldAcreFeet(year?: number): number {
    const waterTransfersForWaterYear = this.getSoldWaterTransfersForWaterYear(year);
    if (waterTransfersForWaterYear.length > 0) {
      return waterTransfersForWaterYear.reduce(function (a, b) {
        return (a + b.AcreFeetTransferred);
      }, 0);
    }
    return null;
  }

  public getTotalSupply(): number {
    return this.getAnnualAllocation() + this.getPurchasedAcreFeet() - this.getSoldAcreFeet();
  }

  public getCurrentAvailableWater(): number {
    return this.getTotalSupply() - this.getWaterUsageToDate();
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


  initializeCharts(waterUsage: WaterUsageDto[], waterUsageOverview: WaterAllocationOverviewDto) {

    this.waterUsageChartData = waterUsage.map(x => {
      return {
        Year: x.Year,
        ChartData: x.WaterUsage
      }
    });


    let values = [];
    for (const series of waterUsageOverview.Current) {
      for (const entry of series.CumulativeWaterUsage) {
        values.push(entry.value);
      }
    }

    this.annualAllocationChartData = this.waterYears.map(x => {
      const allocation = this.getAnnualAllocationForSpecificWaterYear(x);
      const sold = this.getSoldAcreFeet(x);
      const purchased = this.getPurchasedAcreFeet(x);

      values.push(allocation + purchased - sold);
      const months = ["January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"];
      return {
        Year: x,
        ChartData: {
          name: "Annual Supply (Allocation +/- Trades)",
          series: months.map(y => { return { name: y, value: allocation + purchased - sold } })
        }
      }
    })

    this.allocationChartRange = [0, 1.2 * Math.max(...values)];
    this.waterUsageOverview = waterUsageOverview;
    this.historicCumulativeWaterUsage = new MultiSeriesEntry("Average Consumption (All Years)", waterUsageOverview.Historic);
    this.historicAverageAnnualUsage = (waterUsageOverview.Historic.find(x => x.name == "December").value as number).toFixed(1);
  }

  public getWaterUsageForWaterYear(): MultiSeriesEntry[] {
    return this.waterUsageChartData.find(x => x.Year == this.waterYearToDisplay).ChartData;
  }

  public getCumulativeWaterUsageForWaterYear(): SeriesEntry[] {
    let currentYearData = this.waterUsageOverview.Current.find(x => x.Year == this.waterYearToDisplay);
    return currentYearData.CumulativeWaterUsage;
  }

  public getAnnualAllocationSeries(): MultiSeriesEntry {
    return this.annualAllocationChartData.find(x => x.Year == this.waterYearToDisplay).ChartData
  }
}
