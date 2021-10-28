import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
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
import { WaterAllocationOverviewDto } from 'src/app/shared/models/water-usage-dto';
import { MultiSeriesEntry, SeriesEntry } from "src/app/shared/models/series-entry";
import { ParcelLedgerDto } from 'src/app/shared/models/parcel/parcel-ledger-dto';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { AccountSimpleDto } from 'src/app/shared/models/account/account-simple-dto';
import { AccountService } from 'src/app/services/account/account.service';
import { environment } from 'src/environments/environment';
import { LandownerWaterUseChartComponent } from '../landowner-water-use-chart/landowner-water-use-chart.component';
import { WaterYearDto } from "src/app/shared/models/water-year-dto";
import { WaterYearService } from 'src/app/services/water-year.service';
import { LandownerDashboardViewEnum } from 'src/app/shared/models/enums/landowner-dashboard-view.enum';
import { ParcelSimpleDto } from 'src/app/shared/models/parcel/parcel-simple-dto';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { WaterTypeDto } from 'src/app/shared/models/water-type-dto';
import { TransactionTypeEnum } from 'src/app/shared/models/enums/transaction-type-enum';
import { Listener } from 'selenium-webdriver';

@Component({
  selector: 'rio-landowner-dashboard',
  templateUrl: './landowner-dashboard.component.html',
  styleUrls: ['./landowner-dashboard.component.scss']
})
export class LandownerDashboardComponent implements OnInit, OnDestroy {
  @ViewChild('landownerWaterUseChart') landownerWaterUseChart: LandownerWaterUseChartComponent;
  
  public waterYearToDisplay: WaterYearDto;
  public currentUser: UserDto;
  private watchUserChangeSubscription: any;
  private watchAccountChangeSubscription: any;
  public showAcresManagedDetails: boolean;
  public showAllocationDetails: boolean;
  public showPurchasedDetails: boolean;
  public showSoldDetails: boolean;
  public unitsShown: string = "ac-ft";
  public waterUsageByParcelViewType: string = "chart";
  public LandownerDashboardViewEnum = LandownerDashboardViewEnum;
  public sectionCurrentlyViewing: LandownerDashboardViewEnum = LandownerDashboardViewEnum.WaterBudget;
  public hoverItem: string;
  public waterYearDropdownTextColor: string;

  public user: UserDto;
  public account: AccountSimpleDto;
  public accounts: Array<AccountSimpleDto>;
  public parcels: Array<ParcelDto>;
  public parcelNumbers: string[];
  public postings: Array<PostingDto>;
  public trades: Array<TradeWithMostRecentOfferDto>;
  public waterYears: Array<WaterYearDto>;
  public waterTransfers: Array<WaterTransferDto>;
  public currentDate: Date;
  private tradeStatusIDs: TradeStatusEnum[];
  private postingStatusIDs: PostingStatusEnum[];

  private waterUsageChartData: { Year: number, ChartData: MultiSeriesEntry[] };
  private waterUsageOverview: WaterAllocationOverviewDto;
  private historicCumulativeWaterUsage: MultiSeriesEntry;
  private annualAllocationChartData: { Year: number, ChartData: MultiSeriesEntry }[];
  private allocationChartRange: number[];
  public historicAverageAnnualUsage: string | number;
  public parcelLedgers: Array<ParcelLedgerDto>;
  public parcelLedgersForYear: Array<ParcelLedgerDto>;
  public waterUsages: any;
  public activeAccount: AccountSimpleDto;

  public highlightedParcelDto: ParcelDto;
  private _highlightedParcelID: number;
  public loadingActiveAccount: boolean = false;
  parcelsToBeReconciled: ParcelSimpleDto[];
  set highlightedParcelID(value: number) {
    this._highlightedParcelID = value;
    this.highlightedParcelDto = this.parcels.filter(x => x.ParcelID == value)[0];
 }
 
  get highlightedParcelID(): number {     
      return this._highlightedParcelID;    
  }

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
  public waterTypes: WaterTypeDto[];

  public selectedParcelsLayerName: string = "<img src='./assets/main/images/parcel_blue.png' style='height:16px; margin-bottom:3px'> Account Parcels";

  constructor(
    private route: ActivatedRoute,
    private postingService: PostingService,
    private parcelService: ParcelService,
    private tradeService: TradeService,
    private authenticationService: AuthenticationService,
    private waterTypeService: WaterTypeService,
    private cdr: ChangeDetectorRef,
    private accountService: AccountService,
    private waterYearSerivce: WaterYearService
  ) {
  }

  ngOnInit() {
    this.loadingActiveAccount = true;
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.tradeStatusIDs = [TradeStatusEnum.Accepted, TradeStatusEnum.Countered, TradeStatusEnum.Rejected, TradeStatusEnum.Rescinded];
      this.postingStatusIDs = [PostingStatusEnum.Open, PostingStatusEnum.Closed];
      this.showAllocationDetails = false;
      this.showPurchasedDetails = false;
      this.showSoldDetails = false;

      this.currentDate = (new Date());

      this.waterTypeService.getWaterTypes().subscribe(waterTypes => {
        this.waterTypes = waterTypes;
        let accountNumber = parseInt(this.route.snapshot.paramMap.get("accountNumber"));
        if (accountNumber) {
          this.accountService.getAccountByAccountNumber(accountNumber).subscribe(account => {
            this.activeAccount = account instanceof Array
              ? null
              : account as AccountSimpleDto;
              this.loadingActiveAccount = false;
            this.updateAccountData(account);
          });
        }
      })

      this.cdr.detectChanges();      
    }); 
  }

  public getAccountDisplayName(): string {
    return this.activeAccount.AccountDisplayName;
  }

  public getViewEnum(): string[] {
    var values = Object.values(this.LandownerDashboardViewEnum);

    if (!this.allowTrading()) {
      values = values.filter(x => x != LandownerDashboardViewEnum.Trading);
    }

    return values;
  }

  public checkSelectedView(value: string): boolean {
    return this.sectionCurrentlyViewing == value;
  }

  public getImgSrcForSelector(value: string): string {
    var color = this.checkSelectedView(value) || this.hoverItem == value ? "white" : "black";
    return `assets/main/images/landowner-dashboard-view/${value}-${color}.png`;
  }

  public updateView(value: string) {
    var key = Object.keys(LandownerDashboardViewEnum).filter(x => LandownerDashboardViewEnum[x] == value)[0];
    this.sectionCurrentlyViewing = LandownerDashboardViewEnum[key];
  }

  public updateAccountData(account: AccountSimpleDto): void {
    forkJoin(
      this.postingService.getPostingsByAccountID(account.AccountID),
      this.tradeService.getTradeActivityByAccountID(account.AccountID),
      this.waterYearSerivce.getWaterYears(),
      this.waterYearSerivce.getDefaultWaterYearToDisplay(),
      this.accountService.getParcelsInAccountReconciliationByAccountID(account.AccountID)
    ).subscribe(([postings, trades, waterYears, defaultWaterYear, parcelsToBeReconciled]) => {
      this.waterYears = waterYears;
      this.waterYearToDisplay = defaultWaterYear;
      this.postings = postings;
      this.trades = trades;
      this.parcelsToBeReconciled = parcelsToBeReconciled
      this.updateAnnualData();
    });
  }

  public updateAnnualData() {
    if (!this.activeAccount || !this.waterYearToDisplay) {
      return;
    }

    this.parcelService.getParcelsByAccountID(this.activeAccount.AccountID, this.waterYearToDisplay.Year).subscribe(parcels => {
      this.parcels = parcels;
      this.parcelNumbers = Array.from(new Set(parcels.map(x => x.ParcelNumber)));
    });

    forkJoin(
      this.accountService.getParcelLedgersByAccountID(this.activeAccount.AccountID),
      this.accountService.getWaterUsageOverviewByAccountID(this.activeAccount.AccountID, this.waterYearToDisplay.Year)
    ).subscribe(([parcelLedgers, waterUsageOverview]) => {
      this.parcelLedgers = parcelLedgers;
      this.parcelLedgersForYear = parcelLedgers.filter(x => x.WaterYear == this.waterYearToDisplay.Year);
      console.log(waterUsageOverview);
      this.waterUsages = {
          Year: this.waterYearToDisplay.Year,
          AnnualUsage: this.createWaterUsagesForYear(this.waterYearToDisplay.Year)
          };
      // user will have neither allocation nor usage to chart if they have no parcels in this year.
      if (this.parcels && this.parcels.length > 0) {
        this.initializeCharts(waterUsageOverview);
      }

      this.landownerWaterUseChart ? this.landownerWaterUseChart.buildColorScheme() : null;
    })
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.watchAccountChangeSubscription?.unsubscribe();
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
      return "You have no open trades for " + this.waterYearToDisplay?.Year + ". Change the viewing year in the top of the Landowner Dashboard to view open trades for other years.";
    }
    else {
      return "You have no trade activity for " + this.waterYearToDisplay?.Year +". Change the viewing year in the top of the Landowner Dashboard to view open trades for other years.";
    }
  }

  public getNoPostingsMessage(): string {
    if (this.postingStatusIDs.length === 1) {
      return "You have no open postings for " + this.waterYearToDisplay?.Year + ". Change the viewing year in the top of the Landowner Dashboard to view open postings for other years.";
    }
    else {
      return "You have no posting activity for " + this.waterYearToDisplay?.Year + ". Change the viewing year in the top of the Landowner Dashboard to view posting activity for other years.";
    }
  }

  public getPostingsForWaterYear(): Array<PostingDto> {
    return this.postings
      .filter(x => (new Date(x.PostingDate).getFullYear()).toString() === this.waterYearToDisplay?.Year.toString() && this.postingStatusIDs.includes(x.PostingStatus.PostingStatusID))
      .sort((a, b) => a.PostingDate > b.PostingDate ? -1 : a.PostingDate < b.PostingDate ? 1 : 0);
  }

  public getTradesForWaterYear(): Array<TradeWithMostRecentOfferDto> {
    return this.trades ?
      this.trades
        .filter(x => (new Date(x.OfferDate).getFullYear()).toString() === this.waterYearToDisplay?.Year.toString()
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
    let parcelLedgers = this.getAllocationsForWaterYear(year);
    return this.getTotalTransactionAmountForParcelLedgers(parcelLedgers, skipConvertToUnitsShown);
  }

  public getAllocationByWaterType(waterType: WaterTypeDto): number{
    let parcelLedgers = this.getAllocationsForWaterYear(this.waterYearToDisplay.Year).filter(pa => pa.WaterType.WaterTypeID === waterType.WaterTypeID);
    return this.getTotalTransactionAmountForParcelLedgers(parcelLedgers);
  }

  public getResultInUnitsShown(result: number): number {
    if (this.unitsShown === "ac-ft / ac") {
      return result / this.getTotalAPNAcreage();
    }
    else {
      return result;
    }
  }

  public getAllocationsForWaterYear(year: number): Array<ParcelLedgerDto> {
    if (!this.parcelLedgersForYear) {
      return new Array<ParcelLedgerDto>();
    }

    return this.parcelLedgersForYear.filter(p => p.WaterYear === year && p.TransactionType.TransactionTypeID === TransactionTypeEnum.Allocation);
  }

  public getAllocationForParcelAndYear(parcelID: number, year: number): string {
    if (!this.parcelLedgersForYear) {
      return null;
    }
    
    var parcelLedgersForYear = this.getAllocationsForWaterYear(year);
    if (parcelLedgersForYear.length > 0) {
      var parcelLedgersForYearAndParcel = parcelLedgersForYear.filter(p => p.ParcelID == parcelID)
  
      return this.getTotalTransactionAmountForParcelLedgers(parcelLedgersForYearAndParcel).toFixed(1);
    }
    
    return "-";
  }

  public getLastETReadingDate(): string {
    return "12/31/" + this.waterYearToDisplay?.Year; //TODO: need to use the date from the latest monthly ET data
  }

  private getParcelLedgersForWaterYear(year?: number) {
    if (!year) {
      year = this.waterYearToDisplay?.Year
    }
    return this.parcelLedgersForYear.filter(x => x.WaterYear == year);
  }

  public getTradeSalesForWaterYear(year?: number) {
    return this.getParcelLedgersForWaterYear(year).filter(x => x.TransactionType.TransactionTypeID === TransactionTypeEnum.TradeSale);
  }

  public getTradePurchasesForWaterYear(year?: number) {
    return this.getParcelLedgersForWaterYear(year).filter(x => x.TransactionType.TransactionTypeID === TransactionTypeEnum.TradePurchase);
  }

  public isWaterTransferPending(waterTransfer: WaterTransferDto) {
    return !waterTransfer.BuyerRegistration.IsRegistered || !waterTransfer.SellerRegistration.IsRegistered;
  }

  public getPurchasedAcreFeet(year?: number): number {
    return this.getTotalTransactionAmountForParcelLedgers(this.getTradePurchasesForWaterYear(year));
  }

  public getSoldAcreFeet(year?: number, skipConvertToUnitsShown?: boolean): number {
    return this.getTotalTransactionAmountForParcelLedgers(this.getTradeSalesForWaterYear(year), skipConvertToUnitsShown);
  }

  private getTotalTransactionAmountForParcelLedgers(parcelLedgersForWaterYear: ParcelLedgerDto[], skipConvertToUnitsShown?: boolean): number {
    if (parcelLedgersForWaterYear.length > 0) {
      let result = parcelLedgersForWaterYear.reduce(function (a, b) {
        return (a + b.TransactionAmount);
      }, 0);
      if (skipConvertToUnitsShown){
        return result;
      }
      return this.getResultInUnitsShown(result);
    }
    return null;
  }

  public getTotalSupply(): number {
    return this.getAnnualAllocation(this.waterYearToDisplay.Year) + this.getPurchasedAcreFeet() - this.getSoldAcreFeet();
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
    let annualAllocation = this.getAnnualAllocation(this.waterYearToDisplay.Year);
    let estimatedAvailableSupply = this.getWaterUsageToDate();
    if (annualAllocation !== null && estimatedAvailableSupply !== null) {
      return annualAllocation - estimatedAvailableSupply;
    }
    return null;
  }

  initializeCharts(waterUsageOverview: WaterAllocationOverviewDto) {
    this.waterUsageChartData = {
      Year: this.waterYearToDisplay.Year,
      ChartData: this.createParcelMonthlyUsage(this.waterYearToDisplay.Year)
    };
    
    let values = [];
    for (const series of waterUsageOverview.Current) {
      for (const entry of series.CumulativeWaterUsage) {
        values.push(entry.value);
      }
    }

    const allocationLabel = environment.allowTrading ? "Annual Supply (Allocation +/- Trades)" : "Annual Supply"
    
    this.annualAllocationChartData = this.waterYears.map(x => {
      const allocation = this.getAnnualAllocation(x.Year, true);
      const sold = this.getSoldAcreFeet(x.Year, true);
      const purchased = this.getPurchasedAcreFeet(x.Year);

      values.push(allocation + purchased - sold);      

      return {
        Year: x.Year,
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
  
  private createParcelMonthlyUsage(year: number): MultiSeriesEntry[] {
    const parcelLedgers = this.getParcelLedgerUsageForYear(year);
    return this.months.map((x, monthIndex) => 
      {
        return {
          name: x,
          series: parcelLedgers.filter(y => y.WaterMonth === monthIndex + 1)
            .map(z => {
              return {
                name: z.ParcelID.toString(),
                value: -z.TransactionAmount
              };
            })
        };
      });
  }

  private createWaterUsagesForYear(year: number) {
    const totalUsage = this.getParcelLedgerUsageForYear(year).reduce((a, b) => {
      return (a + b.TransactionAmount);
    }, 0);
    return Math.abs(totalUsage);
  }

  private getParcelLedgerUsageForYear(year: number)
  {
    const usageTransactionTypeIDs = [TransactionTypeEnum.MeasuredUsage, TransactionTypeEnum.MeasuredUsageCorrection, TransactionTypeEnum.ManualAdjustment];
    return this.getParcelLedgersForWaterYear(year).filter(x => usageTransactionTypeIDs.includes(x.TransactionType.TransactionTypeID));
  }

  public getWaterUsageForWaterYear(): MultiSeriesEntry[] {
    if (!this.waterUsageChartData) {
      return null;
    }

    return this.waterUsageChartData.ChartData;
  }

  public isWaterUsageForWaterYearNullOrEmpty(): boolean {
    if (!this.waterUsageChartData) {
      return true;
    }

    return this.waterUsageChartData.ChartData.every(x => x.series.every(y => y["IsEmpty"]));
  }

  public getCumulativeWaterUsageForWaterYear(): SeriesEntry[] {
    if (!this.waterUsageOverview) {
      return this.emptyCumulativeWaterUsage
    }

    let currentYearData = this.waterUsageOverview.Current.find(x => x.Year == this.waterYearToDisplay.Year);
    return currentYearData ? currentYearData.CumulativeWaterUsage : this.emptyCumulativeWaterUsage;
  }

  public getAnnualAllocationSeries(): MultiSeriesEntry {
    if (!this.annualAllocationChartData) {
      return null;
    }

    const annualAllocation = this.annualAllocationChartData.find(x => x.Year == this.waterYearToDisplay.Year);
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

  public getParcelNumbersForParcelsToBeReconciled() {
    if (!this.parcelsToBeReconciled) {
      return null;
    }

    return this.parcelsToBeReconciled.map(x => x.ParcelNumber).join(", ");
  }
}
