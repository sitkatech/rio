import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { UserDto } from 'src/app/shared/models';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { TradeService } from 'src/app/services/trade.service';
import { forkJoin } from 'rxjs';
import { ColDef } from 'ag-grid-community';
import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';
import { PostingService } from 'src/app/services/posting.service';
import { FontAwesomeIconLinkRendererComponent } from 'src/app/shared/components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
import { PostingStatusEnum } from 'src/app/shared/models/enums/posting-status-enum';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'rio-manager-dashboard',
  templateUrl: './manager-dashboard.component.html',
  styleUrls: ['./manager-dashboard.component.scss']
})
export class ManagerDashboardComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  public currentUser: UserDto;

  public parcels: Array<ParcelDto>;

  public trades = [];
  public tradesGridColumnDefs: ColDef[];
  public postings = [];
  public postingsGridColumnDefs: ColDef[];
  public landownerUsageReports = [];
  public landownerUsageReportGridColumnDefs: ColDef[];

  public waterYearToDisplay: number;
  public waterYears: Array<number>;

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
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
      this.waterYearToDisplay = 2019;

      forkJoin(
        this.parcelService.getParcelsWithLandOwners(),
        this.tradeService.getAllTradeActivity(),
        this.postingService.getPostingsDetailed(),
        this.userService.getLandowneUsageReportByYear(this.waterYearToDisplay),
      ).subscribe(([parcels, trades, postings, landownerUsageReport]) => {
        this.parcels = parcels;
        this.initializeTradeActivityGrid(trades);
        this.initializePostingActivityGrid(postings);
        this.initializeLandownerUsageReportGrid(landownerUsageReport);
      });
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

  private initializePostingActivityGrid(postings: any, ): void {
    let _currencyPipe = this.currencyPipe;
    let _datePipe = this.datePipe;

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
          return { LinkValue: params.data.PostingID, LinkDisplay: _datePipe.transform(params.data.PostingDate, "short"), PostingDate: params.data.PostingDate };
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
          return { LinkValue: params.data.PostedByUserID, LinkDisplay: params.data.PostedByFullName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/users/" },
        filterValueGetter: function (params: any) {
          return params.data.PostedByFullName;
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
      { headerName: '# of Offers', field: 'NumberOfOffers', sortable: true, filter: true, width: 120 },
      { headerName: 'Price', field: 'Price', valueFormatter: function (params) { return _currencyPipe.transform(params.value, "USD"); }, sortable: true, filter: true, width: 100 },
      { headerName: 'Initial Quantity', field: 'Quantity', sortable: true, filter: true, width: 140 },
      { headerName: 'Available Quantity', field: 'AvailableQuantity', sortable: true, filter: true, width: 160 },
    ];
    this.postings = postings;
  }

  private initializeTradeActivityGrid(trades: any, ): void {
    let _currencyPipe = this.currencyPipe;
    let _datePipe = this.datePipe;

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
          return _datePipe.transform(params.value, "short")
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
          if(params.data.TradeStatus.TradeStatusID === TradeStatusEnum.Accepted)
          {
            if(params.data.BuyerRegistration.IsCanceled || params.data.SellerRegistration.IsCanceled)
            {
              return "Transaction Canceled";
            }
            else if(!params.data.BuyerRegistration.IsRegistered || !params.data.SellerRegistration.IsRegistered)
            {
              return "Awaiting Registration";
            }
            else
            {
              return params.data.TradeStatus.TradeStatusDisplayName;
            }
          }
          return params.data.TradeStatus.TradeStatusDisplayName; },
        sortable: true, filter: true, width: 200
      },
      {
        headerName: 'Buyer', valueGetter: function (params: any) {
          return { LinkValue: params.data.Buyer.UserID, LinkDisplay: params.data.Buyer.FullName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/users/" },
        filterValueGetter: function (params: any) {
          return params.data.Buyer.FullName;
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
        headerName: 'Seller', valueGetter: function (params: any) {
          return { LinkValue: params.data.Seller.UserID, LinkDisplay: params.data.Seller.FullName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/users/" },
        filterValueGetter: function (params: any) {
          return params.data.Seller.FullName;
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
      { headerName: 'Quantity (acre-feet)', field: 'Quantity', sortable: true, filter: true, width: 160 },
      { headerName: 'Unit Price', field: 'Price', valueFormatter: function (params) { return _currencyPipe.transform(params.value, "USD"); }, sortable: true, filter: true, width: 100 },
      { headerName: 'Total Price', valueGetter: function (params) { return params.data.Price * params.data.Quantity; }, valueFormatter: function (params) { return _currencyPipe.transform(params.value, "USD"); }, sortable: true, filter: true, width: 130 },
      {
        headerName: 'Posted By', valueGetter: function (params: any) {
          return { LinkValue: params.data.OfferCreateUser.UserID, LinkDisplay: params.data.OfferCreateUser.FullName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/users/" },
        filterValueGetter: function (params: any) {
          return params.data.OfferCreateUser.FullName;
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
    this.trades = trades;
  }

  private initializeLandownerUsageReportGrid(landownerUsageReport: any, ): void {
    let _decimalPipe = this.decimalPipe;

    this.landownerUsageReportGridColumnDefs = [
      {
        headerName: 'Landowner', valueGetter: function (params: any) {
          return { LinkValue: params.data.UserID, LinkDisplay: params.data.FullName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/users/" },
        filterValueGetter: function (params: any) {
          return params.data.FullName;
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
      { headerName: 'Total Allocation', field: 'Allocation', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 150 },
      { headerName: 'Project Water', field: 'ProjectWater', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 130 },
      { headerName: 'Reconciliation', field: 'Reconciliation', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 130 },
      { headerName: 'Native Yield', field: 'NativeYield', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 130 },
      { headerName: 'Stored Water', field: 'StoredWater', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 130 },
      { headerName: 'Purchased', field: 'Purchased', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.0-0"); }, sortable: true, filter: true, width: 120 },
      { headerName: 'Sold', field: 'Sold', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.0-0"); }, sortable: true, filter: true, width: 100 },
      { headerName: 'Total Supply', field: 'TotalSupply', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 130 },
      { headerName: 'Current Available', field: 'CurrentAvailable', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 150 },
      {
        headerName: 'Most Recent Trade', valueGetter: function (params: any) {
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
        sortable: true, filter: true, width: 160
      },
      { headerName: '# of Trades', field: 'NumberOfTrades', sortable: true, filter: true, width: 120 },
      { headerName: '# of Postings', field: 'NumberOfPostings', sortable: true, filter: true, width: 120 },
    ];
    this.landownerUsageReports = landownerUsageReport;
  }
}
