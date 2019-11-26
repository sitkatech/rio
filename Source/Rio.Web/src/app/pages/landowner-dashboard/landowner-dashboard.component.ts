import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { UserService } from 'src/app/services/user/user.service';
import { ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
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
import { ParcelAllocationDto } from 'src/app/shared/models/parcel/parcel-allocation-dto';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { ParcelAllocationTypeEnum } from 'src/app/shared/models/enums/parcel-allocation-type-enum';


@Component({
  selector: 'rio-landowner-dashboard',
  templateUrl: './landowner-dashboard.component.html',
  styleUrls: ['./landowner-dashboard.component.scss']
})
export class LandownerDashboardComponent implements OnInit, OnDestroy {
  public waterYearToDisplay: number;
  public currentUser: UserDto;
  private watchUserChangeSubscription: any;
  public showAcresManagedDetails: boolean;
  public showAllocationDetails: boolean;
  public showPurchasedDetails: boolean;
  public showSoldDetails: boolean;
  public unitsShown: string = "ac-ft";

  public user: UserDto;
  public parcels: Array<ParcelDto>;
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
  public parcelAllocations: Array<ParcelAllocationDto>;
  public waterUsages: any;

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
      this.tradeStatusIDs = [TradeStatusEnum.Countered];
      this.postingStatusIDs = [PostingStatusEnum.Open];
      this.showAllocationDetails = false;
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
        
        this.postingService.getPostingsByUserID(userID),
        this.tradeService.getTradeActivityForUser(userID),
        this.userService.getWaterTransfersByUserID(userID),
        this.userService.getWaterUsageByUserID(userID),
        this.userService.getWaterUsageOverviewByUserID(userID),
        this.parcelService.getWaterYears()
      ).subscribe(([postings, trades, waterTransfers, waterUsagesInChartForm, waterUsageOverview, waterYears]) => {

        this.waterYears = waterYears;
        this.waterYearToDisplay = (new Date()).getFullYear();
        this.postings = postings;
        this.trades = trades;
        this.waterTransfers = waterTransfers;
        
        this.waterUsages = waterUsagesInChartForm.map(x => 
          {
            return { 
              Year: x.Year, 
              AnnualUsage: 
                x.WaterUsage.map(wu => {
                  return {
                    monthlyValue: wu.series.reduce((a, b) => {
                      return (a + b.value);
                    }, 0)
                  };
                }).reduce((a, b) => {
                  return (a + b.monthlyValue);
                }, 0)
            };
        });
        this.initializeCharts(waterUsagesInChartForm, waterUsageOverview);
      });
      this.cdr.detectChanges();
    });
  }

  public updateAnnualData() {
    this.parcelService.getParcelsByUserID(this.user.UserID, this.waterYearToDisplay).subscribe(parcels=>{
      this.parcels = parcels;
      this.parcelNumbers = Array.from(new Set(parcels.map(x => x.ParcelNumber)));
    });
    
    this.userService.getParcelsAllocationsByUserID(this.user.UserID, this.waterYearToDisplay).subscribe(parcelAllocations => {
      this.parcelAllocations = parcelAllocations;
    })
  }
  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public toggleUnitsShown(): void {
    if (this.unitsShown === "ac-ft") {
      this.unitsShown = "ac-ft / ac";
    }
    else {
      this.unitsShown = "ac-ft";
    }
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
      .filter(x => (new Date(x.PostingDate).getFullYear()).toString() === this.waterYearToDisplay.toString() && this.postingStatusIDs.includes(x.PostingStatus.PostingStatusID))
      .sort((a, b) => a.PostingDate > b.PostingDate ? -1 : a.PostingDate < b.PostingDate ? 1 : 0);
  }

  public getTradesForWaterYear(): Array<TradeWithMostRecentOfferDto> {
    return this.trades ?
      this.trades
        .filter(x => (new Date(x.OfferDate).getFullYear()).toString() === this.waterYearToDisplay.toString() 
        && (this.tradeStatusIDs.includes(x.TradeStatus.TradeStatusID) 
        || (x.OfferStatus.OfferStatusID === OfferStatusEnum.Accepted 
          && !this.isTradeCanceled(x) && (x.BuyerRegistration.IsPending || x.SellerRegistration.IsPending))))
        .sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0) : [];
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

  public isTradeCanceled(trade: TradeWithMostRecentOfferDto) {
    return trade.BuyerRegistration.IsCanceled || trade.SellerRegistration.IsCanceled;
  }

  public isTradeRegisteredByUser(trade: TradeWithMostRecentOfferDto) {
    return (trade.BuyerRegistration.IsRegistered && trade.Buyer.UserID === this.user.UserID) || (trade.SellerRegistration.IsRegistered && trade.Seller.UserID === this.user.UserID);
  }

  public isTradeRegisteredByBothParties(trade: TradeWithMostRecentOfferDto) {
    return trade.BuyerRegistration.IsRegistered && trade.SellerRegistration.IsRegistered;
  }

  public getSelectedParcelIDs(): Array<number> {
    return Array.from(new Set(this.parcels.map((item: any) => item.ParcelID)));
  }

  public getTotalAPNAcreage(): number {
    if (this.parcels.length > 0) {
      let result = this.parcels.reduce(function (a, b) {
        return (a + b.ParcelAreaInAcres);
      }, 0);
      return result;
    }
    return 0;
  }

  public getAnnualAllocation(year: number): number {
    let parcelAllocations = this.getAllocationsForWaterYear(year);
    return this.getTotalAcreFeetAllocated(parcelAllocations);
  }

  public getAnnualProjectWater(): number {
    let parcelAllocations = this.getAllocationsForWaterYear(this.waterYearToDisplay).filter(pa => pa.ParcelAllocationTypeID === ParcelAllocationTypeEnum.ProjectWater);
    return this.getTotalAcreFeetAllocated(parcelAllocations);
  }

  public getAnnualReconciliation(): number {
    let parcelAllocations = this.getAllocationsForWaterYear(this.waterYearToDisplay).filter(pa => pa.ParcelAllocationTypeID === ParcelAllocationTypeEnum.Reconciliation);
    return this.getTotalAcreFeetAllocated(parcelAllocations);
  }

  public getAnnualNativeYield(): number {
    let parcelAllocations = this.getAllocationsForWaterYear(this.waterYearToDisplay).filter(pa => pa.ParcelAllocationTypeID === ParcelAllocationTypeEnum.NativeYield);
    return this.getTotalAcreFeetAllocated(parcelAllocations);
  }

  public getAnnualStoredWater(): number {
    let parcelAllocations = this.getAllocationsForWaterYear(this.waterYearToDisplay).filter(pa => pa.ParcelAllocationTypeID === ParcelAllocationTypeEnum.StoredWater);
    return this.getTotalAcreFeetAllocated(parcelAllocations);
  }

  public getTotalAcreFeetAllocated(parcelAllocations : Array<ParcelAllocationDto>): number {
    var result = 0;
    if (parcelAllocations.length > 0) {
      result = parcelAllocations.reduce(function (a, b) {
        return (a + b.AcreFeetAllocated);
      }, 0);
    }
    return this.getResultInUnitsShown(result);
  }

  public getResultInUnitsShown(result: number) : number
  {
    if(this.unitsShown === "ac-ft / ac")
    {
      return result / this.getTotalAPNAcreage();
    }
    else
    {
      return result;
    }
  }

  public getAllocationsForWaterYear(year: number) {
    return this.parcelAllocations.filter(p => p.WaterYear.toString() === year.toString());
  }

  public getLastETReadingDate(): string {
    return "12/31/" + this.waterYearToDisplay; //TODO: need to use the date from the latest monthly ET data
  }

  private getWaterTransfersForWaterYear(year?: number) {
    if (!year) {
      year = this.waterYearToDisplay
    }
    return this.waterTransfers.filter(x => x.TransferYear == year && !x.BuyerRegistration.IsCanceled && !x.SellerRegistration.IsCanceled);
  }

  public getSoldWaterTransfersForWaterYear(year?: number) {
    return this.getWaterTransfersForWaterYear(year).filter(x => x.SellerRegistration.User.UserID === this.user.UserID);
  }

  public getPurchasedWaterTransfersForWaterYear(year?: number) {
    return this.getWaterTransfersForWaterYear(year).filter(x => x.BuyerRegistration.User.UserID === this.user.UserID);
  }

  public isWaterTransferPending(waterTransfer: WaterTransferDto) {
    return !waterTransfer.BuyerRegistration.IsRegistered || !waterTransfer.SellerRegistration.IsRegistered;
  }

  public getPurchasedAcreFeet(year?: number): number {
    return this.getTradedQuantity(this.getPurchasedWaterTransfersForWaterYear(year));
  }

  public getSoldAcreFeet(year?: number): number {
    return this.getTradedQuantity(this.getSoldWaterTransfersForWaterYear(year));
  }

  private getTradedQuantity(waterTransfersForWaterYear: WaterTransferDto[]) : number
  {
    if (waterTransfersForWaterYear.length > 0) {
      let result = waterTransfersForWaterYear.reduce(function (a, b) {
        return (a + b.AcreFeetTransferred);
      }, 0);
      return this.getResultInUnitsShown(result);
    }
    return null;
  }

  public getTotalSupply(): number {
    return this.getAnnualAllocation(this.waterYearToDisplay) + this.getPurchasedAcreFeet() - this.getSoldAcreFeet();
  }

  public getCurrentAvailableWater(): number {
    return this.getTotalSupply() - this.getWaterUsageToDate();
  }

  public getWaterUsageToDate(): number {
    const waterUsageForYear = this.waterUsages.find(x => x.Year === this.waterYearToDisplay);
    if (waterUsageForYear) {
      return this.getResultInUnitsShown(waterUsageForYear.AnnualUsage);
    }
    return 0;
  }

  public getEstimatedAvailableSupply(): number {
    let annualAllocation = this.getAnnualAllocation(this.waterYearToDisplay);
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
      const allocation = this.getAnnualAllocation(x);
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
    this.historicAverageAnnualUsage = (waterUsageOverview.Historic.find(x => x.name == "December").value as number);
  }

  public getWaterUsageForWaterYear(): MultiSeriesEntry[] {
    const annualWaterUsage = this.waterUsageChartData.find(x => x.Year == this.waterYearToDisplay);
    return annualWaterUsage ? annualWaterUsage.ChartData : null;
  }

  public getCumulativeWaterUsageForWaterYear(): SeriesEntry[] {
    let currentYearData = this.waterUsageOverview.Current.find(x => x.Year == this.waterYearToDisplay);
    return currentYearData ? currentYearData.CumulativeWaterUsage : null;
  }

  public getAnnualAllocationSeries(): MultiSeriesEntry {
    const annualAllocation = this.annualAllocationChartData.find(x => x.Year == this.waterYearToDisplay);
    return annualAllocation ? annualAllocation.ChartData : null;
  }
}
