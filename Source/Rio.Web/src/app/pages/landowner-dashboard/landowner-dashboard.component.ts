import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { TradeService } from 'src/app/services/trade.service';
import { OfferStatusEnum } from 'src/app/shared/models/enums/offer-status-enum';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';
import { PostingService } from 'src/app/services/posting.service';
import { PostingStatusEnum } from 'src/app/shared/models/enums/posting-status-enum';
import { WaterSupplyOverviewDto } from 'src/app/shared/models/water-usage-dto';
import { MultiSeriesEntry, SeriesEntry } from "src/app/shared/models/series-entry";
import { AccountService } from 'src/app/services/account/account.service';
import { environment } from 'src/environments/environment';
import { LandownerWaterUseChartComponent } from '../landowner-water-use-chart/landowner-water-use-chart.component';
import { WaterYearService } from 'src/app/services/water-year.service';
import { LandownerDashboardViewEnum } from 'src/app/shared/models/enums/landowner-dashboard-view.enum';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { TransactionTypeEnum } from 'src/app/shared/models/enums/transaction-type-enum';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { ParcelDto } from 'src/app/shared/generated/model/parcel-dto';
import { ParcelLedgerDto } from 'src/app/shared/generated/model/parcel-ledger-dto';
import { ParcelSimpleDto } from 'src/app/shared/generated/model/parcel-simple-dto';
import { PostingDto } from 'src/app/shared/generated/model/posting-dto';
import { TradeWithMostRecentOfferDto } from 'src/app/shared/generated/model/trade-with-most-recent-offer-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTransferDto } from 'src/app/shared/generated/model/water-transfer-dto';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { WaterYearDto } from 'src/app/shared/generated/model/water-year-dto';
import { WaterTransferDetailedDto } from 'src/app/shared/generated/model/water-transfer-detailed-dto';
import { ParcelLedgerEntrySourceTypeEnum } from 'src/app/shared/models/enums/parcel-ledger-entry-source-type-enum';
import { ColDef, ColumnApi, GridApi } from 'ag-grid-community';
import { AgGridAngular } from 'ag-grid-angular';
import { DatePipe } from '@angular/common';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { ParcelLedgerEntrySourceTypeDto } from 'src/app/shared/generated/model/parcel-ledger-entry-source-type-dto';

@Component({
  selector: 'rio-landowner-dashboard',
  templateUrl: './landowner-dashboard.component.html',
  styleUrls: ['./landowner-dashboard.component.scss']
})
export class LandownerDashboardComponent implements OnInit, OnDestroy {
  @ViewChild('landownerWaterUseChart') landownerWaterUseChart: LandownerWaterUseChartComponent;
  @ViewChild('activityDetailGrid') activityDetailGrid: AgGridAngular;
  
  public waterYearToDisplay: WaterYearDto;
  public currentUser: UserDto;
  
  private watchAccountChangeSubscription: any;
  public showAcresManagedDetails: boolean;
  public showSupplyDetails: boolean;
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
  private waterUsageOverview: WaterSupplyOverviewDto;
  private historicCumulativeWaterUsage: MultiSeriesEntry;
  private annualWaterSupplyChartData: { Year: number, ChartData: MultiSeriesEntry }[];
  private waterSupplyChartRange: number[];
  public historicAverageAnnualUsage: string | number;
  public parcelLedgers: Array<ParcelLedgerDto>;
  public parcelLedgersForWaterYear: Array<ParcelLedgerDto>;
  public parcelLedgersBalance: Map<number, number>;
  public waterUsages: any;
  public activeAccount: AccountSimpleDto;
  public activityDisplayCount = 5;
  public displayAccountActivityDetailed: boolean = false;
 
  public activityDetailGridColDefs: ColDef[];
  public defaultColDef: ColDef;
  private columnApi: ColumnApi;
  private gridApi: GridApi;

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

  public emptyCumulativeWaterUsage: SeriesEntry[] = this.months.map(y => { return this.createSeriesEntry(y, 0); });
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
    private waterYearService: WaterYearService,
    private utilityFunctionsService: UtilityFunctionsService,
    private datePipe: DatePipe
  ) {
  }

  ngOnInit() {
    this.loadingActiveAccount = true;
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
      this.tradeStatusIDs = [TradeStatusEnum.Accepted, TradeStatusEnum.Countered, TradeStatusEnum.Rejected, TradeStatusEnum.Rescinded];
      this.postingStatusIDs = [PostingStatusEnum.Open, PostingStatusEnum.Closed];
      this.showSupplyDetails = false;

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
      });

      this.createActivityDetailGridColDefs();

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
      this.waterYearService.getWaterYears(),
      this.waterYearService.getDefaultWaterYearToDisplay(),
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

    if (!this.parcelLedgers) {
      this.accountService.getParcelLedgersByAccountID(this.activeAccount.AccountID).subscribe(parcelLedgers => {
        this.parcelLedgers = parcelLedgers;
        // call needs to wait until this.parcelLedgers is populated to avoid race condition
        this.updateParcelLedgersAndChartDataForWaterYear();
      });
    } else {
      this.updateParcelLedgersAndChartDataForWaterYear();
    }    
  }

  private updateParcelLedgersAndChartDataForWaterYear():void {
    this.parcelLedgersForWaterYear = this.getParcelLedgersForWaterYear();
    this.parcelLedgersBalance = this.createParcelLedgersBalance();

    if (this.displayAccountActivityDetailed) {
      this.gridApi.setRowData(this.parcelLedgersForWaterYear);
    }
    
    this.waterUsages = {
      Year: this.waterYearToDisplay.Year,
      AnnualUsage: this.createWaterUsagesForYear(this.waterYearToDisplay.Year)
    };
    // user will have neither water supply nor usage to chart if they have no parcels in this year.
    if (this.parcels && this.parcels.length > 0) {
      this.initializeCharts();
    }
    this.landownerWaterUseChart ? this.landownerWaterUseChart.buildColorScheme() : null;
  }

  ngOnDestroy() {
    
    this.watchAccountChangeSubscription?.unsubscribe();
    
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

  public getResultInUnitsShown(result: number): number {
    if (this.unitsShown === "ac-ft / ac") {
      return result / this.getTotalAPNAcreage();
    }
    else {
      return result;
    }
  }

  public getLastETReadingDate(): string {
    return "12/31/" + this.waterYearToDisplay?.Year; //TODO: need to use the date from the latest monthly ET data
  }

  private getParcelLedgersForWaterYear(year?: number) {
    if (!this.parcelLedgers) {
      return new Array<ParcelLedgerDto>();
    }

    if (!year) {
      year = this.waterYearToDisplay?.Year
    }

    return this.parcelLedgers.filter(x => x.WaterYear == year);
  }

  public getAnnualWaterSupply(year: number, skipConvertToUnitsShown?: boolean): number {
    let parcelLedgers = this.getWaterSupplyForWaterYear(year);
    return this.getTotalTransactionAmountForParcelLedgers(parcelLedgers, skipConvertToUnitsShown);
  }

  public getWaterSupplyByWaterType(waterType: WaterTypeDto): number{
    let parcelLedgers = this.getWaterSupplyForWaterYear(this.waterYearToDisplay.Year).filter(p => p.WaterType?.WaterTypeID === waterType.WaterTypeID);
    return this.getTotalTransactionAmountForParcelLedgers(parcelLedgers) ?? 0;
  }

  public getPrecipWaterSupply(): number {
    let parcelLedgers = this.getWaterSupplyForWaterYear(this.waterYearToDisplay.Year).filter(pa => pa.ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeID === ParcelLedgerEntrySourceTypeEnum.CIMIS);
    return this.getTotalTransactionAmountForParcelLedgers(parcelLedgers) ?? 0;
  }

  public getTotalWaterSupplyForParcelAndYear(parcelID: number, year: number): string {
    var parcelLedgersForYear = this.getWaterSupplyForWaterYear(year);
    if (parcelLedgersForYear.length > 0) {
      var parcelLedgersForYearAndParcel = parcelLedgersForYear.filter(p => p.Parcel.ParcelID == parcelID)
  
      return this.getTotalTransactionAmountForParcelLedgers(parcelLedgersForYearAndParcel).toFixed(1);
    }
    
    return "-";
  }

  public getWaterSupplyForWaterYear(year: number): Array<ParcelLedgerDto> {
    let parcelLedgers = this.getParcelLedgersForWaterYear(year).filter(p => 
      p.TransactionType.TransactionTypeID === TransactionTypeEnum.Supply && 
      (p.ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeID === ParcelLedgerEntrySourceTypeEnum.Manual ||
      p.ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeID === ParcelLedgerEntrySourceTypeEnum.CIMIS)
    );
    return parcelLedgers;
  }

  public getTradeSalesForWaterYear(year?: number) {
    return this.getParcelLedgersForWaterYear(year).filter(x => 
      x.ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeID === ParcelLedgerEntrySourceTypeEnum.Trade && 
      x.TransactionAmount < 0);
  }

  public getTradePurchasesForWaterYear(year?: number) {
    return this.getParcelLedgersForWaterYear(year).filter(x => 
      x.ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeID === ParcelLedgerEntrySourceTypeEnum.Trade && 
      x.TransactionAmount > 0);
  }

  public isWaterTransferPending(waterTransfer: WaterTransferDetailedDto) {
    return !waterTransfer.BuyerRegistration.IsRegistered || !waterTransfer.SellerRegistration.IsRegistered;
  }

  public getPurchasedWaterSupply(year?: number): number {
    return this.getTotalTransactionAmountForParcelLedgers(this.getTradePurchasesForWaterYear(year)) ?? 0;
  }

  public getSoldWaterSupply(year?: number, skipConvertToUnitsShown?: boolean): number {
    return this.getTotalTransactionAmountForParcelLedgers(this.getTradeSalesForWaterYear(year), skipConvertToUnitsShown) ?? 0;
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
    return this.getAnnualWaterSupply(this.waterYearToDisplay.Year) + this.getPurchasedWaterSupply() + this.getSoldWaterSupply();
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
    let annualWaterSupply = this.getAnnualWaterSupply(this.waterYearToDisplay.Year);
    let estimatedAvailableSupply = this.getWaterUsageToDate();
    if (annualWaterSupply !== null && estimatedAvailableSupply !== null) {
      return annualWaterSupply - estimatedAvailableSupply;
    }
    return null;
  }

  public updateActivityDisplayCount() {
    this.activityDisplayCount += 5;
  }

  private createActivityDetailGridColDefs() {
    this.activityDetailGridColDefs = [
      { 
        headerName: 'APN', field: 'Parcel.ParcelNumber', valueGetter: function (params: any) {
          return { LinkValue: params.data.Parcel.ParcelID, LinkDisplay: params.data.Parcel.ParcelNumber }
        }, filterValueGetter: function (params: any) {
          return params.data.Parcel.ParcelNumber;
        }, cellRendererFramework: LinkRendererComponent, cellRendererParams: { 'inRouterLink': '/parcels/' }
      },
      this.createDateColumnDef('Effective Date', 'EffectiveDate', 'M/d/yyyy'),
      this.createDateColumnDef('Transaction Date', 'TransactionDate', 'short'),
      { headerName: 'Transaction Type', field: 'TransactionType.TransactionTypeName'},
      {
        headerName: 'Supply Type', valueGetter: function (params: any) {
          return params.data.WaterType ? params.data.WaterType.WaterTypeName : '-';
        }
      },
      { headerName: 'Source Type', field: 'ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeDisplayName'},
      { 
        headerName: 'Quantity (ac-ft)', filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right'},
        valueGetter: function (params: any) { return parseFloat(params.data.TransactionAmount.toFixed(2)); }, 
      },
      { headerName: 'Transaction Description', field: 'TransactionDescription', sortable: false },
      { headerName: 'Comment', field: 'UserComment', sortable: false,
        valueGetter: function (params: any) {
          return params.data.UserComment ?? '-';
        }
      }
    ];

    this.defaultColDef = {
      sortable: true, 
      filter: true, 
      resizable: true
    }
  }

  private dateFilterComparator(filterLocalDateAtMidnight, cellValue) {
    const filterDate = Date.parse(filterLocalDateAtMidnight);
    const cellDate = Date.parse(cellValue);

    if (cellDate == filterDate) {
      return 0;
    }
    return (cellDate < filterDate) ? -1 : 1;
  }

  private dateSortComparer (id1: any, id2: any) {
    const date1 = id1 ? Date.parse(id1) : Date.parse("1/1/1900");
    const date2 = id2 ? Date.parse(id2) : Date.parse("1/1/1900");
    if (date1 < date2) {
      return -1;
    }
    return (date1 > date2)  ?  1 : 0;
}

private createDateColumnDef(headerName: string, fieldName: string, dateFormat: string): ColDef {
  const _datePipe = this.datePipe;
  return {
    headerName: headerName, valueGetter: function (params: any) {
      return _datePipe.transform(params.data[fieldName], dateFormat);
    },
    comparator: this.dateSortComparer,
    filter: 'agDateColumnFilter',
    filterParams: {
      filterOptions: ['inRange'],
      comparator: this.dateFilterComparator
    }, 
    width: 110,
    resizable: true,
    sortable: true
  };
}

  public onGridReady(params: any) {
    this.columnApi = params.columnApi;
    this.gridApi = params.api;

    this.gridApi.setRowData(this.parcelLedgersForWaterYear);
    this.columnApi.autoSizeAllColumns();

  }

  public exportActivityDetailGridToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.activityDetailGrid, this.waterYearToDisplay.Year + 'ActivityforAccount' + this.activeAccount.AccountNumber + '.csv', null);
  }
  

  initializeCharts() {
    if (!this.waterUsageOverview) {
      this.waterUsageOverview = this.createParcelUsageOverviewChartData();
    }
   
    this.waterUsageChartData = {
      Year: this.waterYearToDisplay.Year,
      ChartData: this.createParcelMonthlyUsageChartData(this.waterYearToDisplay.Year)
    };

    let values = [];
    for (const series of this.waterUsageOverview.Current) {
      for (const entry of series.CumulativeWaterUsage) {
        values.push(entry.value);
      }
    }

    const waterSupplyLabel = environment.allowTrading ? "Annual Supply (Water Supply +/- Trades)" : "Annual Supply"
    
    this.annualWaterSupplyChartData = this.waterYears.map(x => {
      const waterSupply = this.getAnnualWaterSupply(x.Year, true);
      const sold = this.getSoldWaterSupply(x.Year, true);
      const purchased = this.getPurchasedWaterSupply(x.Year);

      values.push(waterSupply + purchased - sold);      

      return {
        Year: x.Year,
        ChartData: {
          name: waterSupplyLabel,
          series: this.months.map(y => { return this.createSeriesEntry(y, waterSupply + purchased - sold) })
        }
      }
    });

    this.waterSupplyChartRange = [0, 1.2 * Math.max(...values)];
    this.historicCumulativeWaterUsage = new MultiSeriesEntry("Average Consumption (All Years)", this.waterUsageOverview.Historic);
    this.historicAverageAnnualUsage = (this.waterUsageOverview.Historic.find(x => x.name == this.months[11]).value as number);
  }
  
  private createSeriesEntry(name: string, value: number, isEmpty?: boolean): SeriesEntry {
    const seriesEntry = new SeriesEntry(name, value, isEmpty);
    return seriesEntry;
  }

  private createParcelMonthlyUsageChartData(year: number): MultiSeriesEntry[] {
    const parcelLedgers = this.getParcelLedgerUsageForYear(year);
    
    // create sparsly populated matrix of usages by parcel and month
    let usageByParcelAndMonth = {};

    for (let parcel of this.parcels) {
      usageByParcelAndMonth[parcel.ParcelNumber] = {};
      for (let i = 1; i < 13; i++) {
        usageByParcelAndMonth[parcel.ParcelNumber][i] = null;
      }
    }
    
    for (let parcelLedger of parcelLedgers) {
      if (!usageByParcelAndMonth[parcelLedger.Parcel.ParcelNumber][parcelLedger.WaterMonth]) {
        usageByParcelAndMonth[parcelLedger.Parcel.ParcelNumber][parcelLedger.WaterMonth] = 0;
      }

      usageByParcelAndMonth[parcelLedger.Parcel.ParcelNumber][parcelLedger.WaterMonth] += parcelLedger.TransactionAmount;
    }

    return this.months.map((month, monthIndex) => {
        return {
          name: month,
          series: this.parcels.map(parcel => {
            const isEmpty = (usageByParcelAndMonth[parcel.ParcelNumber][monthIndex + 1] === null);
            const seriesValue = isEmpty ? 0 : Math.abs(usageByParcelAndMonth[parcel.ParcelNumber][monthIndex + 1]);

            return this.createSeriesEntry(parcel.ParcelNumber, seriesValue, isEmpty);
          })
        };
      });
  }

  private createParcelUsageOverviewChartData(): WaterSupplyOverviewDto {
    let usageParcelLedgers = this.parcelLedgers.filter(x => x.TransactionType.TransactionTypeID == TransactionTypeEnum.Usage);
    
    // create sparsly populated matrix of usages by year and month, get multiannual monthly usage sums and for determining average monthly usages
    let usageByYearAndMonth: Object = {};
    let historicUsageSums = {};

    for (let waterYear of this.waterYears) {
      let cumulativeMonthlyUsage = 0;
      usageByYearAndMonth[waterYear.Year] = {};

      for (let i = 1; i < 13; i++) {
        let parcelLedgersForMonth = usageParcelLedgers.filter(x => x.WaterYear === waterYear.Year && x.WaterMonth === i);
        let isEmpty = (parcelLedgersForMonth.length == 0);

        if (!isEmpty) {
          cumulativeMonthlyUsage = parcelLedgersForMonth.reduce((a, b) => {
            return a + b.TransactionAmount;
          }, cumulativeMonthlyUsage);
          
          if (!historicUsageSums[i]) {
            historicUsageSums[i] = { usageSum: cumulativeMonthlyUsage, monthsWithDataCount: 1}
          } else {
            historicUsageSums[i]['usageSum'] += cumulativeMonthlyUsage;
            historicUsageSums[i]['monthsWithDataCount']++;
          }
        }
        usageByYearAndMonth[waterYear.Year][i] = isEmpty ? null : cumulativeMonthlyUsage;
      }
    }

    return {
      Current: this.waterYears.map(y => 
        {
          return {
            Year: y.Year, 
            CumulativeWaterUsage: this.months.map((month, i) => 
            {
              let monthlyUsageValue = usageByYearAndMonth[y.Year][i + 1];
              let isEmpty = (monthlyUsageValue == null);

              return this.createSeriesEntry(month, isEmpty ? 0 : Math.abs(monthlyUsageValue), isEmpty);
            })
          }
        }),
      Historic: this.months.map((month, i) => 
      {
        const seriesValue = Math.abs(historicUsageSums[i + 1]['usageSum'] / historicUsageSums[i + 1]['monthsWithDataCount']);
        return this.createSeriesEntry(month, seriesValue);
      })
    }
  }

  private createParcelLedgersBalance(): Map<number, number> {
    const map = new Map();
    let currentBalance = this.parcelLedgersForWaterYear.reduce((a, b) => {
      return a + b.TransactionAmount;
    }, 0);

    for (let parcelLedger of this.parcelLedgersForWaterYear) {
      map.set(parcelLedger.ParcelLedgerID, currentBalance);
      currentBalance -= parcelLedger.TransactionAmount;
    }
    
    return map;
  }

  private createWaterUsagesForYear(year: number): number {
    let usageSum = this.getParcelLedgerUsageForYear(year).reduce((a, b) => {
      return (a + b.TransactionAmount);
    }, 0);
    return Math.abs(usageSum);
  }

  private getParcelLedgerUsageForYear(year: number): ParcelLedgerDto[]
  {
    return this.getParcelLedgersForWaterYear(year).filter(x => x.TransactionType.TransactionTypeID === TransactionTypeEnum.Usage);
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

  public getAnnualWaterSupplySeries(): MultiSeriesEntry {
    if (!this.annualWaterSupplyChartData) {
      return null;
    }

    const annualWaterSupply = this.annualWaterSupplyChartData.find(x => x.Year == this.waterYearToDisplay.Year);
    return annualWaterSupply ? annualWaterSupply.ChartData : null;
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
