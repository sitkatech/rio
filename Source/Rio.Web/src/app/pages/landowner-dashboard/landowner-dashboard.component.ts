import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
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
import { AccountSimpleDto } from 'src/app/shared/models/account/account-simple-dto';
import { AccountService } from 'src/app/services/account/account.service';
import { environment } from 'src/environments/environment';
import { LandownerWaterUseChartComponent } from '../landowner-water-use-chart/landowner-water-use-chart.component';
import { ParcelAllocationTypeDto } from 'src/app/shared/models/parcel-allocation-type-dto';
import { ParcelAllocationTypeService } from 'src/app/services/parcel-allocation-type.service';

@Component({
  selector: 'rio-landowner-dashboard',
  templateUrl: './landowner-dashboard.component.html',
  styleUrls: ['./landowner-dashboard.component.scss']
})
export class LandownerDashboardComponent implements OnInit, OnDestroy {
  @ViewChild('landownerWaterUseChart') landownerWaterUseChart: LandownerWaterUseChartComponent;
  
  public waterYearToDisplay: number;
  public currentUser: UserDto;
  private watchUserChangeSubscription: any;
  private watchAccountChangeSubscription: any;
  public showAcresManagedDetails: boolean;
  public showAllocationDetails: boolean;
  public showPurchasedDetails: boolean;
  public showSoldDetails: boolean;
  public unitsShown: string = "ac-ft";
  public waterUsageByParcelViewType: string = "chart";

  public user: UserDto;
  public account: AccountSimpleDto;
  public accounts: Array<AccountSimpleDto>;
  public parcels: Array<ParcelDto>;
  public parcelNumbers: string[];
  public postings: Array<PostingDto>;
  public trades: Array<TradeWithMostRecentOfferDto>;
  public waterYears: Array<number>;
  public currentDate: Date;
  public waterTransfers: Array<WaterTransferDto>;
  private tradeStatusIDs: TradeStatusEnum[];
  private postingStatusIDs: PostingStatusEnum[];

  private waterUsageChartData: { Year: number, ChartData: MultiSeriesEntry[] };
  private waterUsageOverview: WaterAllocationOverviewDto;
  private historicCumulativeWaterUsage: MultiSeriesEntry;
  private annualAllocationChartData: { Year: number, ChartData: MultiSeriesEntry }[];
  private allocationChartRange: number[];
  public historicAverageAnnualUsage: string | number;
  public parcelAllocations: Array<ParcelAllocationDto>;
  public waterUsages: any;
  public activeAccount: AccountSimpleDto;

  public months = ["Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Oct",
        "Nov",
        "Dec"];

  public emptyCumulativeWaterUsage: SeriesEntry[] = this.months.map(y => { return { name: y, value: 0 } });
  public parcelAllocationTypes: ParcelAllocationTypeDto[];

  constructor(
    private route: ActivatedRoute,
    private postingService: PostingService,
    private parcelService: ParcelService,
    private tradeService: TradeService,
    private authenticationService: AuthenticationService,
    private parcelAllocationTypeService: ParcelAllocationTypeService,
    private cdr: ChangeDetectorRef,
    private accountService: AccountService
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

      this.parcelAllocationTypeService.getParcelAllocationTypes().subscribe(parcelAllocationTypes => {
        this.parcelAllocationTypes = parcelAllocationTypes;
        let accountID = parseInt(this.route.snapshot.paramMap.get("id"));
        if (accountID) {
          this.accountService.getAccountByID(accountID).subscribe(account => {
            this.activeAccount = account instanceof Array
              ? null
              : account as AccountSimpleDto;
            this.setActiveAccount(this.activeAccount);
            this.subscribeToActiveAccount();
          });
        } else {
          this.subscribeToActiveAccount();
        }
      })

      this.cdr.detectChanges();      
    });   
  }

  public setActiveAccount(account: AccountSimpleDto) {
    this.authenticationService.setActiveAccount(account);
  }

  private subscribeToActiveAccount() {
    this.watchAccountChangeSubscription = this.authenticationService.getActiveAccount().subscribe((account: AccountSimpleDto) => {
      if (account) {
        this.activeAccount = account;
        this.updateAccountData(account);
      }
    });
  }

  public getAccountDisplayName(): string {
    return this.activeAccount.AccountDisplayName;
  }

  public updateAccountData(account: AccountSimpleDto): void {
    forkJoin(
      this.postingService.getPostingsByAccountID(account.AccountID),
      this.tradeService.getTradeActivityByAccountID(account.AccountID),
      this.accountService.getWaterTransfersByAccountID(account.AccountID),
      this.parcelService.getWaterYears(),
      this.parcelService.getDefaultWaterYearToDisplay()
    ).subscribe(([postings, trades, waterTransfers, waterYears, defaultWaterYear]) => {
      this.waterYears = waterYears;
      this.waterYearToDisplay = defaultWaterYear;
      this.postings = postings;
      this.trades = trades;
      this.waterTransfers = waterTransfers;

      this.updateAnnualData();
    });
  }

  public updateAnnualData() {
    if (!this.activeAccount || !this.waterYearToDisplay) {
      return;
    }

    this.parcelService.getParcelsByAccountID(this.activeAccount.AccountID, this.waterYearToDisplay).subscribe(parcels => {
      this.parcels = parcels;
      this.parcelNumbers = Array.from(new Set(parcels.map(x => x.ParcelNumber)));
    });

    this.accountService.getParcelsAllocationsByAccountID(this.activeAccount.AccountID, this.waterYearToDisplay).subscribe(parcelAllocations => {
      this.parcelAllocations = parcelAllocations;
    });

    forkJoin(
      this.accountService.getWaterUsageByAccountID(this.activeAccount.AccountID, this.waterYearToDisplay),
      this.accountService.getWaterUsageOverviewByAccountID(this.activeAccount.AccountID, this.waterYearToDisplay)
    ).subscribe(([waterUsagesInChartForm, waterUsageOverview]) => {
      this.waterUsages = {
          Year: waterUsagesInChartForm.Year,
          AnnualUsage:
          waterUsagesInChartForm.WaterUsage.map(wu => {
              return {
                monthlyValue: wu.series.reduce((a, b) => {
                  return (a + b.value);
                }, 0)
              };
            }).reduce((a, b) => {
              return (a + b.monthlyValue);
            }, 0)
          };

      // user will have neither allocation nor usage to chart if they have no parcels in this year.
      if (this.parcels && this.parcels.length > 0) {
        this.initializeCharts(waterUsagesInChartForm, waterUsageOverview);
      }

      this.landownerWaterUseChart ? this.landownerWaterUseChart.buildColorScheme() : null;
    })
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.watchAccountChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public userHasParcels(): boolean {
    return this.parcels && this.parcels.length > 0
  }

  public toggleUnitsShown(units : string): void {
    this.unitsShown = units;
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
      return "You have no open trades for " + this.waterYearToDisplay + ". Change the viewing year in the top of the Landowner Dashboard to view open trades for other years.";
    }
    else {
      return "You have no trade activity for " + this.waterYearToDisplay +". Change the viewing year in the top of the Landowner Dashboard to view open trades for other years.";
    }
  }

  public getNoPostingsMessage(): string {
    if (this.postingStatusIDs.length === 1) {
      return "You have no open postings for " + this.waterYearToDisplay + ". Change the viewing year in the top of the Landowner Dashboard to view open postings for other years.";
    }
    else {
      return "You have no posting activity for " + this.waterYearToDisplay + ". Change the viewing year in the top of the Landowner Dashboard to view posting activity for other years.";
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
    return trade.OfferCreateAccount.AccountID === this.activeAccount.AccountID;
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
    return (trade.BuyerRegistration.IsRegistered && trade.Buyer.AccountID === this.activeAccount.AccountID) || (trade.SellerRegistration.IsRegistered && trade.Seller.AccountID === this.activeAccount.AccountID);
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

  public getAnnualAllocation(year: number, skipConvertToUnitsShown?: boolean): number {
    let parcelAllocations = this.getAllocationsForWaterYear(year);
    return this.getTotalAcreFeetAllocated(parcelAllocations, skipConvertToUnitsShown);
    
  }

  public getAllocationByAllocationType(parcelAllocationType: ParcelAllocationTypeDto): number{
    let parcelAllocations = this.getAllocationsForWaterYear(this.waterYearToDisplay).filter(pa => pa.ParcelAllocationTypeID === parcelAllocationType.ParcelAllocationTypeID);
    return this.getTotalAcreFeetAllocated(parcelAllocations);
  }

  public getTotalAcreFeetAllocated(parcelAllocations: Array<ParcelAllocationDto>, skipConvertToUnitsShown?: boolean): number {
    var result = 0;
    if (parcelAllocations.length > 0) {
      result = parcelAllocations.reduce(function (a, b) {
        return (a + b.AcreFeetAllocated);
      }, 0);
    }
    if (skipConvertToUnitsShown){
      return result
    }
    return this.getResultInUnitsShown(result);
  }

  public getResultInUnitsShown(result: number): number {
    if (this.unitsShown === "ac-ft / ac") {
      return result / this.getTotalAPNAcreage();
    }
    else {
      return result;
    }
  }

  public getAllocationsForWaterYear(year: number): Array<ParcelAllocationDto> {
    if (!this.parcelAllocations) {
      return new Array<ParcelAllocationDto>();
    }

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
    return this.getWaterTransfersForWaterYear(year).filter(x => x.SellerRegistration.Account.AccountID === this.activeAccount.AccountID);
  }

  public getPurchasedWaterTransfersForWaterYear(year?: number) {
    return this.getWaterTransfersForWaterYear(year).filter(x => x.BuyerRegistration.Account.AccountID === this.activeAccount.AccountID);
  }

  public isWaterTransferPending(waterTransfer: WaterTransferDto) {
    return !waterTransfer.BuyerRegistration.IsRegistered || !waterTransfer.SellerRegistration.IsRegistered;
  }

  public getPurchasedAcreFeet(year?: number): number {
    return this.getTradedQuantity(this.getPurchasedWaterTransfersForWaterYear(year));
  }

  public getSoldAcreFeet(year?: number, skipConvertToUnitsShown?: boolean): number {
    return this.getTradedQuantity(this.getSoldWaterTransfersForWaterYear(year), skipConvertToUnitsShown);
  }

  private getTradedQuantity(waterTransfersForWaterYear: WaterTransferDto[], skipConvertToUnitsShown?: boolean): number {
    if (waterTransfersForWaterYear.length > 0) {
      let result = waterTransfersForWaterYear.reduce(function (a, b) {
        return (a + b.AcreFeetTransferred);
      }, 0);
      if (skipConvertToUnitsShown){
        return result;
      }
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
    if (!this.waterUsages) {
      return null;
    }
    
    return this.getResultInUnitsShown(this.waterUsages.AnnualUsage);
  }

  public getEstimatedAvailableSupply(): number {
    let annualAllocation = this.getAnnualAllocation(this.waterYearToDisplay);
    let estimatedAvailableSupply = this.getWaterUsageToDate();
    if (annualAllocation !== null && estimatedAvailableSupply !== null) {
      return annualAllocation - estimatedAvailableSupply;
    }
    return null;
  }

  initializeCharts(waterUsage: WaterUsageDto, waterUsageOverview: WaterAllocationOverviewDto) {
    this.waterUsageChartData = {
      Year: waterUsage.Year,
      ChartData: waterUsage.WaterUsage      
    };

    let values = [];
    for (const series of waterUsageOverview.Current) {
      for (const entry of series.CumulativeWaterUsage) {
        values.push(entry.value);
      }
    }

    const allocationLabel = environment.allowTrading ? "Annual Supply (Allocation +/- Trades)" : "Annual Supply"
    
    this.annualAllocationChartData = this.waterYears.map(x => {
      const allocation = this.getAnnualAllocation(x, true);
      const sold = this.getSoldAcreFeet(x, true);
      const purchased = this.getPurchasedAcreFeet(x);

      values.push(allocation + purchased - sold);      

      return {
        Year: x,
        ChartData: {
          name: allocationLabel,
          series: this.months.map(y => { return { name: y, value: allocation + purchased - sold } })
        }
      }
    });

    this.allocationChartRange = [0, 1.2 * Math.max(...values)];
    this.waterUsageOverview = waterUsageOverview;
    this.historicCumulativeWaterUsage = new MultiSeriesEntry("Average Consumption (All Years)", waterUsageOverview.Historic);
    this.historicAverageAnnualUsage = (waterUsageOverview.Historic.find(x => x.name == this.months[11]).value as number);
  }

  public getWaterUsageForWaterYear(): MultiSeriesEntry[] {
    if (!this.waterUsageChartData) {
      return null;
    }

    return this.waterUsageChartData.ChartData;
  }

  public getCumulativeWaterUsageForWaterYear(): SeriesEntry[] {
    if (!this.waterUsageOverview) {
      return this.emptyCumulativeWaterUsage
    }

    let currentYearData = this.waterUsageOverview.Current.find(x => x.Year == this.waterYearToDisplay);
    return currentYearData ? currentYearData.CumulativeWaterUsage : this.emptyCumulativeWaterUsage;
  }

  public getAnnualAllocationSeries(): MultiSeriesEntry {
    if (!this.annualAllocationChartData) {
      return null;
    }

    const annualAllocation = this.annualAllocationChartData.find(x => x.Year == this.waterYearToDisplay);
    return annualAllocation ? annualAllocation.ChartData : null;
  }

  public allowTrading(): boolean {
    return environment.allowTrading;
  }

  public toggleWaterUsageByParcelView(): void {
    if (this.waterUsageByParcelViewType === "chart"){
      this.waterUsageByParcelViewType = "table";
    }
    else {
      this.waterUsageByParcelViewType = "chart";
    }
  }

  public getTotalWaterUsageForMonth(monthNum : number) : number {
    let currentWaterUsage = this.getWaterUsageForWaterYear();
    let sum = 0;
    if (currentWaterUsage != null && currentWaterUsage != undefined) {
      currentWaterUsage[monthNum].series.forEach(element => {
        sum += element.value;
      });
    }
    return sum;
  }

  public getTotalWaterUsageForParcel(parcelNum : string) : number {
    let currentWaterUsage = this.getWaterUsageForWaterYear();
    let sum = 0;
    if (currentWaterUsage != null && currentWaterUsage != undefined) {
      currentWaterUsage.forEach(month => {
        month.series.forEach(parcelUsage => {
          if (parcelUsage.name === parcelNum) {
            sum += parcelUsage.value;
          }
        })
      })
    }
    return sum;
  }

  public landownerHasAnyAccounts(){
    var result = this.authenticationService.getAvailableAccounts();
    return result != null && result.length > 0;
  }
}
