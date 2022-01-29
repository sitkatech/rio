import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { TradeService } from 'src/app/services/trade.service';
import { ColDef } from 'ag-grid-community';
import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';
import { PostingService } from 'src/app/services/posting.service';
import { FontAwesomeIconLinkRendererComponent } from 'src/app/shared/components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
import { PostingStatusEnum } from 'src/app/shared/models/enums/posting-status-enum';
import { UserService } from 'src/app/services/user/user.service';
import { AgGridAngular } from 'ag-grid-angular';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { environment } from 'src/environments/environment';
import { forkJoin } from 'rxjs';
import { MultiSeriesEntry, SeriesEntry } from 'src/app/shared/models/series-entry';
import { AccountService } from 'src/app/services/account/account.service';
import { WaterSupplyOverviewDto } from 'src/app/shared/models/water-usage-dto';
import { WaterYearService } from 'src/app/services/water-year.service';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { ParcelDto } from 'src/app/shared/generated/model/parcel-dto';
import { PostingDetailedDto } from 'src/app/shared/generated/model/posting-detailed-dto';
import { TradeWithMostRecentOfferDto } from 'src/app/shared/generated/model/trade-with-most-recent-offer-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { WaterYearDto } from 'src/app/shared/generated/model/water-year-dto';
import { LandownerUsageReportDto } from 'src/app/shared/generated/model/landowner-usage-report-dto';

@Component({
  selector: 'rio-manager-dashboard',
  templateUrl: './manager-dashboard.component.html',
  styleUrls: ['./manager-dashboard.component.scss']
})
export class ManagerDashboardComponent implements OnInit, OnDestroy {
  @ViewChild('landOwnerUsageReportGrid') landOwnerUsageReportGrid: AgGridAngular;
  @ViewChild('tradeActivityGrid') tradeActivityGrid: AgGridAngular;
  @ViewChild('postingsGrid') postingsGrid: AgGridAngular;
  public modalReference: NgbModalRef;


  
  public currentUser: UserDto;

  public parcels: Array<ParcelDto>;
  public landownerUsageReport: Array<LandownerUsageReportDto>;

  public tradesGridColumnDefs: ColDef[];
  public postingsGridColumnDefs: ColDef[];
  public landownerUsageReportGridColumnDefs: ColDef[];

  public waterYearToDisplay: WaterYearDto;
  public waterYears: Array<WaterYearDto>;
  public unitsShown: string = "ac-ft";
  public displayTradeGrid: boolean = false;
  public displayPostingsGrid: boolean = false;
  public waterTypes: WaterTypeDto[];
  public waterTypesBatched: WaterTypeDto[][];
  private waterTypeColDefsInsertIndex: number;
  private tradeColDefsInsertIndex: number;
  public tradeActivity: TradeWithMostRecentOfferDto[];
  public waterSupplyLabel: string = "Annual Water Supply";
  postingActivity: PostingDetailedDto[];

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
  private annualWaterSupplyChartData: { Year: number, ChartData: MultiSeriesEntry }[];

  public selectedParcelsLayerName: string = "<img src='./assets/main/images/parcel_blue.png' style='height:16px; margin-bottom:3px'> Account Parcels";
  public waterUsageOverview: WaterSupplyOverviewDto;
  public waterSupplyChartRange: number[];
  public historicCumulativeWaterUsage: MultiSeriesEntry;
  public historicAverageAnnualUsage: number;

  public isPerformingAction: boolean = false;

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private tradeService: TradeService,
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
    private postingService: PostingService,
    private waterTypeService: WaterTypeService,
    private userService: UserService,
    private currencyPipe: CurrencyPipe,
    private decimalPipe: DecimalPipe,
    private accountService: AccountService,
    private datePipe: DatePipe,
    private modalService: NgbModal,
    private alertService: AlertService,
    ) { }

  ngOnInit() {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
      this.initializeTradeActivityGrid();
      this.initializePostingActivityGrid();
      this.initializeLandownerUsageReportGrid();

      forkJoin(this.waterYearService.getDefaultWaterYearToDisplay(),
        this.waterYearService.getWaterYears(),
        this.waterTypeService.getWaterTypes()
      ).subscribe(([defaultYear, waterYears, waterTypes]) => {
        this.waterYearToDisplay = defaultYear;
        this.waterYears = waterYears;
        this.waterTypes = waterTypes;
        this.waterTypesBatched = this.getWaterTypesBatched();

        let decimalPipe = this.decimalPipe;
        const newLandownerUsageReportGridColumnDefs: ColDef[] = [];
        // define column defs for water types
        this.waterTypes.forEach(waterType => {
          const waterTypeFieldName = 'WaterSupplyByWaterType.' + waterType.WaterTypeID;
          newLandownerUsageReportGridColumnDefs.push(
            this.utilityFunctionsService.createDecimalColumnDef(waterType.WaterTypeName, waterTypeFieldName, 130)
          )
        });

        this.landownerUsageReportGridColumnDefs.splice(this.waterTypeColDefsInsertIndex, 0, ...newLandownerUsageReportGridColumnDefs)

        this.landownerUsageReportGridColumnDefs.forEach(x => {
          x.resizable = true;
        });
        
        this.updateAnnualData();
      });     
    });    
  }

  ngOnDestroy() {
    
    
    this.cdr.detach();
  }

  public getSelectedParcelIDs(): Array<number> {
    return this.parcels !== undefined ? this.parcels.map(p => p.ParcelID) : [];
  }

  initializeCharts(waterUsageOverview: WaterSupplyOverviewDto) {
    let values = [];
    for (const series of waterUsageOverview.Current) {
      for (const entry of series.CumulativeWaterUsage) {
        values.push(entry.value);
      }
    }
    
    this.annualWaterSupplyChartData = this.waterYears.map(x => {
      const waterSupply = this.getAnnualWaterSupply(true);
      values.push(waterSupply);

      return {
        Year: x.Year,
        ChartData: {
          name: this.waterSupplyLabel,
          series: this.months.map(y => { return { name: y, value: waterSupply } })
        }
      }
    });

    this.waterSupplyChartRange = [0, 1.2 * Math.max(...values)];
    this.historicCumulativeWaterUsage = new MultiSeriesEntry("Average Consumption (All Years)", waterUsageOverview.Historic);
    this.historicAverageAnnualUsage = (waterUsageOverview.Historic.find(x => x.name == this.months[11]).value as number);
  }

  public getAnnualWaterSupplySeries(): MultiSeriesEntry {
    if (!this.annualWaterSupplyChartData) {
      return null;
    }

    const annualWaterSupply = this.annualWaterSupplyChartData.find(x => x.Year == this.waterYearToDisplay.Year);
    return annualWaterSupply ? annualWaterSupply.ChartData : null;
  }

  public getCumulativeWaterUsageForWaterYear(): SeriesEntry[] {
    if (!this.waterUsageOverview) {
      return this.emptyCumulativeWaterUsage
    }

    let currentYearData = this.waterUsageOverview.Current.find(x => x.Year == this.waterYearToDisplay.Year);
    return currentYearData ? currentYearData.CumulativeWaterUsage : this.emptyCumulativeWaterUsage;
  }

  private initializePostingActivityGrid(): void {
    let _currencyPipe = this.currencyPipe;
    let _datePipe = this.datePipe;
    let _decimalPipe = this.decimalPipe;

    this.postingsGridColumnDefs = [
      {
        headerName: '', valueGetter:
          function (params: any) {
            if (params.data.NumberOfOffers === 0 && params.data.PostingStatusID === PostingStatusEnum.Open) {
              return params.data.PostingID;
            }
            return null;
          }
        , cellRendererFramework: FontAwesomeIconLinkRendererComponent,
        cellRendererParams: { inRouterLink: "/delete-posting", fontawesomeIconName: 'trash' },
        sortable: false, filter: false, width: 30
      },
      {
        headerName: 'Date', valueGetter: function (params: any) {
          return { LinkValue: params.data.PostingID, LinkDisplay: _datePipe.transform(params.data.PostingDate, "M/d/yyyy, h:mm a"), PostingDate: params.data.PostingDate };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/postings/" },
        filterValueGetter: function (params: any) {
          return _datePipe.transform(params.data.PostingDate, "M/d/yyyy");
        },
        filterParams: {
          // provide comparator function
          comparator: function (filterLocalDate, cellValue) {
            var dateAsString = cellValue;
            if (dateAsString == null) return -1;
            var cellDate = Date.parse(dateAsString);
            const filterLocalDateAtMidnight = filterLocalDate.getTime();
            if (filterLocalDateAtMidnight == cellDate) {
              return 0;
            }
            if (cellDate < filterLocalDateAtMidnight) {
              return -1;
            }
            if (cellDate > filterLocalDateAtMidnight) {
              return 1;
            }
          }
        },
        comparator: function (id1: any, id2: any) {
          if (id1.PostingDate < id2.PostingDate) {
            return -1;
          }
          if (id1.PostingDate > id2.PostingDate) {
            return 1;
          }
          return 0;
        },
        sortable: true, filter: 'agDateColumnFilter', width: 140
      },
      {
        headerName: 'Posted By (User)', valueGetter: function (params: any) {
          return { LinkValue: params.data.PostedByUserID ?? '', LinkDisplay: (params.data.PostedByFullName !== " " ? params.data.PostedByFullName : 'User Not Found') };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/users/" },
        filterValueGetter: function (params: any) {
          return params.data.PostedByFullName !== " " ? params.data.PostedByFullName : 'User Not Found';
        },
        comparator: function (id1: any, id2: any) {
          let link1 = id1.LinkDisplay;
          let link2 = id2.LinkDisplay;
          if (link1 < link2) {
            return -1;
          }
          if (link1 > link2) {
            return 1;
          }
          return 0;
        },
        sortable: true, filter: true, width: 155
      },
      {
        headerName: 'Posted By (Account)', valueGetter: function (params: any) {
          return { LinkValue: params.data.PostedByAccountID ?? '', LinkDisplay: (params.data.PostedByAccountName !== " " ? params.data.PostedByAccountName : 'User Not Found') };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/accounts/" },
        filterValueGetter: function (params: any) {
          return params.data.PostedByAccountName !== " " ? params.data.PostedByAccountName : 'Account Not Found';
        },
        comparator: function (id1: any, id2: any) {
          let link1 = id1.LinkDisplay;
          let link2 = id2.LinkDisplay;
          if (link1 < link2) {
            return -1;
          }
          if (link1 > link2) {
            return 1;
          }
          return 0;
        },
        sortable: true, filter: true, width: 155
      },
      { headerName: 'Status', field: 'PostingStatusDisplayName', sortable: true, filter: true, width: 100 },
      { headerName: 'Type', field: 'PostingTypeDisplayName', sortable: true, filter: true, width: 100 },
      { headerName: '# of Offers', field: 'NumberOfOffers', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.0-0"); }, sortable: true, filter: true, width: 120 },
      { headerName: 'Price', field: 'Price', valueFormatter: function (params) { return _currencyPipe.transform(params.value, "USD"); }, sortable: true, filter: true, width: 100 },
      this.utilityFunctionsService.createDecimalColumnDef('Initial Quantity (ac-ft)', 'Quantity', 160, 0),
      this.utilityFunctionsService.createDecimalColumnDef('Available Quantity (ac-ft)', 'AvailableQuantity', 180, 0),
    ];
  }

  private initializeTradeActivityGrid(): void {
    let _currencyPipe = this.currencyPipe;
    let _datePipe = this.datePipe;
    let _decimalPipe = this.decimalPipe;

    this.tradesGridColumnDefs = [
      {
        headerName: 'Trade', valueGetter: function (params: any) {
          return { LinkValue: params.data.TradeNumber, LinkDisplay: params.data.TradeNumber };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/trades/" },
        filterValueGetter: function (params: any) {
          return params.data.LinkValue;
        },
        comparator: function (id1: any, id2: any) {
          let link1 = id1.LinkDisplay;
          let link2 = id2.LinkDisplay;
          if (link1 < link2) {
            return -1;
          }
          if (link1 > link2) {
            return 1;
          }
          return 0;
        },
        sortable: true, filter: true, width: 140
      },
      this.utilityFunctionsService.createDateColumnDef('Date', 'OfferDate', 'M/d/yyyy, h:mm a', 140),
      {
        headerName: 'Status',
        valueGetter: function (params) {
          if (params.data.TradeStatus.TradeStatusID === TradeStatusEnum.Accepted) {
            if (params.data.BuyerRegistration.IsCanceled || params.data.SellerRegistration.IsCanceled) {
              return "Transaction Canceled";
            }
            else if (!params.data.BuyerRegistration.IsRegistered || !params.data.SellerRegistration.IsRegistered) {
              return "Awaiting Registration";
            }
            else {
              return params.data.TradeStatus.TradeStatusDisplayName;
            }
          }
          return params.data.TradeStatus.TradeStatusDisplayName;
        },
        sortable: true, filter: true, width: 200
      },
      {
        headerName: 'Buyer Account', valueGetter: function (params: any) {
          return { LinkValue: params.data.Buyer.AccountNumber, LinkDisplay: params.data.Buyer.ShortAccountDisplayName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/landowner-dashboard/" },
        filterValueGetter: function (params: any) {
          return params.data.Buyer.ShortAccountDisplayName;
        },
        comparator: function (id1: any, id2: any) {
          let link1 = id1.LinkDisplay;
          let link2 = id2.LinkDisplay;
          if (link1 < link2) {
            return -1;
          }
          if (link1 > link2) {
            return 1;
          }
          return 0;
        },
        sortable: true, filter: true, width: 155
      },
      {
        headerName: 'Seller Account', valueGetter: function (params: any) {
          return { LinkValue: params.data.Seller.AccountNumber, LinkDisplay: params.data.Seller.ShortAccountDisplayName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/landowner-dashboard/" },
        filterValueGetter: function (params: any) {
          return params.data.Seller.ShortAccountDisplayName;
        },
        comparator: function (id1: any, id2: any) {
          let link1 = id1.LinkDisplay;
          let link2 = id2.LinkDisplay;
          if (link1 < link2) {
            return -1;
          }
          if (link1 > link2) {
            return 1;
          }
          return 0;
        },
        sortable: true, filter: true, width: 155
      },
      this.utilityFunctionsService.createDecimalColumnDef('Quantity (ac-ft)', 'Quantity', 140, 0),
      { headerName: 'Unit Price', field: 'Price', valueFormatter: function (params) { return _currencyPipe.transform(params.value, "USD"); }, sortable: true, filter: true, width: 100 },
      { headerName: 'Total Price', valueGetter: function (params) { return params.data.Price * params.data.Quantity; }, valueFormatter: function (params) { return _currencyPipe.transform(params.value, "USD"); }, sortable: true, filter: true, width: 130 },
      {
        headerName: 'Posted By', valueGetter: function (params: any) {
          return { LinkValue: params.data.OfferCreateAccountUser?.UserID ?? '', LinkDisplay: (params.data.OfferCreateAccountUser !== null && params.data.OfferCreateAccountUser.FullName !== " " ? params.data.OfferCreateAccountUser.FullName : 'User Not Found') };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/users/" },
        filterValueGetter: function (params: any) {
          return params.data.OfferCreateAccountUser !== null && params.data.OfferCreateAccountUser.FullName !== " " ? params.data.OfferCreateAccountUser.FullName : 'User Not Found';
        },
        comparator: function (id1: any, id2: any) {
          let link1 = id1.LinkDisplay;
          let link2 = id2.LinkDisplay;
          if (link1 < link2) {
            return -1;
          }
          if (link1 > link2) {
            return 1;
          }
          return 0;
        },
        sortable: true, filter: true, width: 155
      },
    ];
        
    this.tradesGridColumnDefs.forEach(x => {
      x.resizable = true;
    });
  }

  private initializeLandownerUsageReportGrid(): void {
    let _decimalPipe = this.decimalPipe;

    // N.B.: After the WaterTypes are retrieved, their column defs will be built and inserted at this index...
    this.waterTypeColDefsInsertIndex = 3;
    this.tradeColDefsInsertIndex = 4;

    this.landownerUsageReportGridColumnDefs = [
      {
        headerName: 'Account Name', valueGetter: function (params: any) {
          return { LinkValue: params.data.AccountID, LinkDisplay: params.data.AccountName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/accounts/" },
        filterValueGetter: function (params: any) {
          return params.data.AccountName;
        },
        comparator: function (id1: any, id2: any) {
          let link1 = id1.LinkDisplay;
          let link2 = id2.LinkDisplay;
          if (link1 < link2) {
            return -1;
          }
          if (link1 > link2) {
            return 1;
          }
          return 0;
        },
        sortable: true, filter: true, width: 155
      },
      { headerName: 'Account Number', field:'AccountNumber', sortable:true, filter: true, width: 155},
      this.utilityFunctionsService.createDecimalColumnDef('Total Supply (ac-ft)', 'TotalSupply', 150),
      // N.B.: The columns for individual water types will be inserted here via a splice after the WaterTypes are retrieved.
      //
      this.utilityFunctionsService.createDecimalColumnDef('Precipitation', 'Precipitation', 170),
      this.utilityFunctionsService.createDecimalColumnDef('Total Usage (ac-ft)', 'UsageToDate', 150),
      this.utilityFunctionsService.createDecimalColumnDef('Current Available (ac-ft)', 'CurrentAvailable', 180),
      this.utilityFunctionsService.createDecimalColumnDef('Acres Managed', 'AcresManaged', 140),
    ];

    // insert trading-related colDefs if trading is enabled
    if (this.allowTrading()) {
      this.landownerUsageReportGridColumnDefs.push(
        this.utilityFunctionsService.createDecimalColumnDef('# of Trades', 'NumberOfTrades', 120, 0),
        this.utilityFunctionsService.createDecimalColumnDef('# of Postings', 'NumberOfPostings', 120, 0),
        {
          headerName: 'Most Recent Trade', valueGetter: function (params: any) {
            return { LinkValue: params.data.MostRecentTradeNumber, LinkDisplay: params.data.MostRecentTradeNumber };
          }, cellRendererFramework: LinkRendererComponent,
          cellRendererParams: { inRouterLink: "/trades/" },
          filterValueGetter: function (params: any) {
            return params.data.LinkValue;
          },
          comparator: function (id1: any, id2: any) {
            let link1 = id1.LinkDisplay;
            let link2 = id2.LinkDisplay;
            if (link1 < link2) {
              return -1;
            }
            if (link1 > link2) {
              return 1;
            }
            return 0;
          },
          sortable: true, filter: true, width: 160
        }
      );

      const purchasedSoldColDefs: Array<ColDef> = [
        this.utilityFunctionsService.createDecimalColumnDef('Purchased (ac-ft)', 'Purchased', 140),
        this.utilityFunctionsService.createDecimalColumnDef('Sold (ac-ft)', 'Sold', 140)
      ];
      this.landownerUsageReportGridColumnDefs.splice(this.tradeColDefsInsertIndex, 0, ...purchasedSoldColDefs);
    }
  }

  public updateAnnualData() {
    if (!this.waterYearToDisplay)
    {
      return;
    }
    this.parcelService.getParcelsWithLandOwners(this.waterYearToDisplay.Year).subscribe(parcels => {
      this.parcels = parcels;
    });
    if (this.landOwnerUsageReportGrid) {
      this.landOwnerUsageReportGrid?.api.showLoadingOverlay();
    }
    this.userService.getLandownerUsageReportByYear(this.waterYearToDisplay.Year).subscribe(landownerUsageReport => {
      this.landownerUsageReport = landownerUsageReport;
      if (!this.landOwnerUsageReportGrid) {
        return;
      }
      this.landOwnerUsageReportGrid.api.setRowData(landownerUsageReport);
      this.landOwnerUsageReportGrid.api.hideOverlay();
      this.getTradesAndPostingsForYear();
      this.accountService.getWaterUsageOverview(this.waterYearToDisplay.Year).subscribe(waterUsageOverview => {
        this.waterUsageOverview = waterUsageOverview;
        this.initializeCharts(this.waterUsageOverview);
      });
    });
  }

  private getTradesAndPostingsForYear() {
    this.tradeService.getTradeActivityForYear(this.waterYearToDisplay.Year).subscribe(result => {
      this.tradeActivity = result;
      this.tradeActivityGrid ? this.tradeActivityGrid.api.setRowData(result) : null;
      this.displayTradeGrid = result.length > 0 ? true : false;
    });
    this.postingService.getPostingsDetailedByYear(this.waterYearToDisplay.Year).subscribe(result => {
      this.postingActivity = result;
      this.postingsGrid ? this.postingsGrid.api.setRowData(result) : null;
      this.displayPostingsGrid = result.length > 0 ? true : false;
    });
  }

  public exportLandOwnerUsageReportToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.landOwnerUsageReportGrid, 'landOwnerReportFor' + this.waterYearToDisplay.Year + '.csv', null);
  }

  public exportTradeActivityToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.tradeActivityGrid, 'tradeActivity.csv', null);
  }

  public exportPostingsToCsv() {
    // we need to grab all columns except the first one (trash icon)
    let columnsKeys = this.postingsGrid.columnApi.getAllDisplayedColumns(); 
    let columnIds: Array<any> = []; 
    columnsKeys.forEach(keys => 
      { 
        let columnName: string = keys.getColId(); 
        if (columnName != '0' && columnName) 
        { 
          columnIds.push(columnName); 
        } 
      });
    this.utilityFunctionsService.exportGridToCsv(this.postingsGrid, 'postings.csv', columnIds);
  }

  public toggleUnitsShown(units : string): void {
    this.unitsShown = units;
  }

  public getAnnualWaterSupply(skipConvertToUnitsShown?: boolean): number {
    return this.getSumOfLandownerUsageReportValuesByField("TotalSupply", skipConvertToUnitsShown);
  }

  public getAnnualUsage(): number {
    return this.getSumOfLandownerUsageReportValuesByField("UsageToDate");
  }

  public getAnnualWaterSupplyByWaterType(waterType: WaterTypeDto): number {
    var result = 0;
    if (this.landownerUsageReport) {
      result = this.landownerUsageReport.reduce(function (a, b) {
        return (a + (b.WaterSupplyByWaterType ? (b.WaterSupplyByWaterType[waterType.WaterTypeID] ?? 0) : 0));
      }, 0);
    }
    return this.getResultInUnitsShown(result);
  }

  public getPrecipWaterSupply(): number {
    return this.getSumOfLandownerUsageReportValuesByField("Precipitation");
  }

  public getPurchasedWaterSupply(): number {
    return this.getSumOfLandownerUsageReportValuesByField("Purchased");
  }

  public getSoldWaterSupply(): number {
    return this.getSumOfLandownerUsageReportValuesByField("Sold");
  }

  public getSumOfLandownerUsageReportValuesByField(field: string, skipConvertToUnitsShown?: boolean): number {
    
    var result = 0;
    if (this.landownerUsageReport) {
      result = this.landownerUsageReport.reduce(function (a, b) {
        return (a + b[field]);
      }, 0);
    }
    if (skipConvertToUnitsShown) {
      return result;
    }
    return this.getResultInUnitsShown(result);
  }

  public getResultInUnitsShown(result: number) : number
  {
    if (this.unitsShown === "ac-ft / ac") {
      return result / this.getTotalAPNAcreage();
    }
    else {
      return result;
    }
  }

  public getTotalAPNAcreage(): number {
    return this.getSumOfLandownerUsageReportValuesByField("AcresManaged", true);
  }

  // batch the water types into twos for the district wide statistics panel
  public getWaterTypesBatched(): WaterTypeDto[][]{
    const batched = [];
    let copy = [...this.waterTypes];
    const batches = Math.ceil(copy.length / 2);
    
    for (let i = 0; i< batches; i++){
      batched.push(copy.splice(0,2));
    }

    return batched;
  }

  public allowTrading():boolean{
    return environment.allowTrading;
  }

  public isAdministrator() : boolean
  {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }

  public launchModal(modalContent: any) {
    this.modalReference = this.modalService.open(modalContent, { windowClass: 'modal-size', backdrop: 'static', keyboard: false });
  }

  public deleteAllTradeActivity() {
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }

    this.isPerformingAction = true;
    this.tradeService.deleteAllTrades().subscribe(response => {
      this.isPerformingAction = false;
      this.alertService.pushAlert(new Alert(`Successfully deleted all Trade activity`, AlertContext.Success));
      this.getTradesAndPostingsForYear();
    },
    error => {
      this.isPerformingAction = false;
      this.alertService.pushAlert(new Alert(`An error occurred while attempting to delete all Trade activity.`, AlertContext.Danger));
      window.scrollTo(0, 0);
    })
  }
  
}
