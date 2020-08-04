import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { UserDto } from 'src/app/shared/models';
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
import { ParcelAllocationAndUsageDto } from 'src/app/shared/models/parcel/parcel-allocation-and-usage-dto';
import { environment } from 'src/environments/environment';

declare var $:any;
@Component({
  selector: 'rio-manager-dashboard',
  templateUrl: './manager-dashboard.component.html',
  styleUrls: ['./manager-dashboard.component.scss']
})
export class ManagerDashboardComponent implements OnInit, OnDestroy {
  @ViewChild('landOwnerUsageReportGrid') landOwnerUsageReportGrid: AgGridAngular;
  @ViewChild('tradeActivityGrid') tradeActivityGrid: AgGridAngular;
  @ViewChild('postingsGrid') postingsGrid: AgGridAngular;

  private watchUserChangeSubscription: any;
  public currentUser: UserDto;

  public parcels: Array<ParcelDto>;
  public parcelAllocationAndUsages: Array<ParcelAllocationAndUsageDto>;

  public tradesGridColumnDefs: ColDef[];
  public postingsGridColumnDefs: ColDef[];
  public landownerUsageReportGridColumnDefs: ColDef[];

  public waterYearToDisplay: number;
  public waterYears: Array<number>;
  public unitsShown: string = "ac-ft";
  public displayTradeGrid: boolean = false;
  public displayPostingsGrid: boolean = false;

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private tradeService: TradeService,
    private parcelService: ParcelService,
    private postingService: PostingService,
    private userService: UserService,
    private currencyPipe: CurrencyPipe,
    private decimalPipe: DecimalPipe,
    private datePipe: DatePipe) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.initializeTradeActivityGrid();
      this.initializePostingActivityGrid();
      this.initializeLandownerUsageReportGrid();

      this.parcelService.getDefaultWaterYearToDisplay().subscribe(defaultYear=>{
        this.waterYearToDisplay = defaultYear;
        this.parcelService.getWaterYearsIncludingCurrentYear().subscribe(waterYears =>{
          this.waterYears = waterYears;
          this.parcelService.getParcelsWithLandOwners(this.waterYearToDisplay).subscribe(parcels=>{
            this.parcels = parcels;
            this.updateAnnualData();        
          })
        }) 
      })     
    });    
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public getSelectedParcelIDs(): Array<number> {
    return this.parcels !== undefined ? this.parcels.map(p => p.ParcelID) : [];
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
        headerName: 'Posted By', valueGetter: function (params: any) {
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
      { headerName: 'Status', field: 'PostingStatusDisplayName', sortable: true, filter: true, width: 100 },
      { headerName: 'Type', field: 'PostingTypeDisplayName', sortable: true, filter: true, width: 100 },
      { headerName: '# of Offers', field: 'NumberOfOffers', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.0-0"); }, sortable: true, filter: true, width: 120 },
      { headerName: 'Price', field: 'Price', valueFormatter: function (params) { return _currencyPipe.transform(params.value, "USD"); }, sortable: true, filter: true, width: 100 },
      { headerName: 'Initial Quantity (ac-ft)', field: 'Quantity', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.0-0"); }, sortable: true, filter: true, width: 160 },
      { headerName: 'Available Quantity (ac-ft)', field: 'AvailableQuantity', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.0-0"); }, sortable: true, filter: true, width: 180 },
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
      {
        headerName: 'Date', field: 'OfferDate', valueFormatter: function (params) {
          return _datePipe.transform(params.value, "M/d/yyyy, h:mm a")
        },
        filterValueGetter: function (params: any) {
          return _datePipe.transform(params.data.OfferDate, "M/d/yyyy");
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
          if (id1.value < id2.value) {
            return -1;
          }
          if (id1.value > id2.value) {
            return 1;
          }
          return 0;
        },
        sortable: true, filter: 'agDateColumnFilter', width: 140
      },
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
          return { LinkValue: params.data.Buyer.AccountID, LinkDisplay: params.data.Buyer.ShortAccountDisplayName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/accounts/" },
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
          return { LinkValue: params.data.Seller.AccountID, LinkDisplay: params.data.Seller.ShortAccountDisplayName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/accounts/" },
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
      { headerName: 'Quantity (ac-ft)', field: 'Quantity', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.0-0"); }, sortable: true, filter: true, width: 140 },
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
      { headerName: 'Total Allocation (ac-ft)', field: 'Allocation', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 170 },
      { headerName: 'Native Yield (ac-ft)', field: 'NativeYield', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 150 },
      { headerName: 'Project Water (ac-ft)', field: 'ProjectWater', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 160 },
      { headerName: 'Stored Water (ac-ft)', field: 'StoredWater', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 150 },
      { headerName: 'Reconciliation (ac-ft)', field: 'Reconciliation', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 160 },
      { headerName: 'Purchased (ac-ft)', field: 'Purchased', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.0-0"); }, sortable: true, filter: true, width: 140 },
      { headerName: 'Sold (ac-ft)', field: 'Sold', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.0-0"); }, sortable: true, filter: true, width: 100 },
      { headerName: 'Total Supply (ac-ft)', field: 'TotalSupply', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 150 },
      { headerName: 'Total Usage (ac-ft)', field: 'UsageToDate', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 150 },
      { headerName: 'Current Available (ac-ft)', field: 'CurrentAvailable', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 180 },
      { headerName: 'Acres Managed', field: 'AcresManaged', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 140 },
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
      },
      { headerName: '# of Trades', field: 'NumberOfTrades', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.0-0"); }, sortable: true, filter: true, width: 120 },
      { headerName: '# of Postings', field: 'NumberOfPostings', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.0-0"); }, sortable: true, filter: true, width: 120 },
    ];
  }

  public updateAnnualData() {
    if (!this.waterYearToDisplay)
    {
      return;
    }
    
    this.parcelService.getParcelsWithLandOwners(this.waterYearToDisplay).subscribe(parcels=>{
      this.parcels = parcels;
    })
    this.userService.getLandownerUsageReportByYear(this.waterYearToDisplay).subscribe(result => {
      this.landOwnerUsageReportGrid ? this.landOwnerUsageReportGrid.api.setRowData(result) : null;
    });
    this.tradeService.getTradeActivityForYear(this.waterYearToDisplay).subscribe(result => {
      this.tradeActivityGrid ? this.tradeActivityGrid.api.setRowData(result) : null;
      this.displayTradeGrid = result.length > 0 ? true : false;
    });
    this.postingService.getPostingsDetailedByYear(this.waterYearToDisplay).subscribe(result => {
      this.postingsGrid ? this.postingsGrid.api.setRowData(result) : null;
      this.displayPostingsGrid = result.length > 0 ? true : false;
    });
    this.parcelService.getParcelAllocationAndUsagesByYear(this.waterYearToDisplay).subscribe(result => {
      this.parcelAllocationAndUsages = result;
    });
  }

  public exportLandOwnerUsageReportToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.landOwnerUsageReportGrid, 'landOwnerReportFor' + this.waterYearToDisplay + '.csv', null);
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

  public getAnnualAllocation(year: number): number {
    let parcelAllocations = this.parcelAllocationAndUsages;
    return this.getTotalAcreFeetAllocated(parcelAllocations, "Allocation");
  }

  public getAnnualUsage(year: number): number {
    let parcelAllocations = this.parcelAllocationAndUsages;
    return this.getTotalAcreFeetAllocated(parcelAllocations, "UsageToDate");
  }

  public getAnnualProjectWater(): number {
    let parcelAllocations = this.parcelAllocationAndUsages;
    return this.getTotalAcreFeetAllocated(parcelAllocations, "ProjectWater");
  }

  public getAnnualReconciliation(): number {
    let parcelAllocations = this.parcelAllocationAndUsages;
    return this.getTotalAcreFeetAllocated(parcelAllocations, "Reconciliation");
  }

  public getAnnualNativeYield(): number {
    let parcelAllocations = this.parcelAllocationAndUsages;
    return this.getTotalAcreFeetAllocated(parcelAllocations, "NativeYield");
  }

  public getAnnualStoredWater(): number {
    let parcelAllocations = this.parcelAllocationAndUsages;
    return this.getTotalAcreFeetAllocated(parcelAllocations, "StoredWater");
  }

  public getTotalAcreFeetAllocated(parcelAllocations : Array<ParcelAllocationAndUsageDto>, field: string): number {
    var result = 0;
    if (parcelAllocations.length > 0) {
      result = parcelAllocations.reduce(function (a, b) {
        return (a + b[field]);
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

  public getTotalAPNAcreage(): number {
    if (this.parcelAllocationAndUsages.length > 0) {
      let result = this.parcelAllocationAndUsages.reduce(function (a, b) {
        return (a + b.ParcelAreaInAcres);
      }, 0);
      return result;
    }
    return 0;
  }

  public allowTrading():boolean{
    return environment.allowTrading;
  }

  public isAdministrator() : boolean
  {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }
}
