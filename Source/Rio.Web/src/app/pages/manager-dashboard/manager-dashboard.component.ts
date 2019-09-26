import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { UserDto } from 'src/app/shared/models';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { TradeService } from 'src/app/services/trade.service';
import { forkJoin } from 'rxjs';
import { ColDef } from 'ag-grid-community';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';
import { PostingService } from 'src/app/services/posting.service';
import { FontAwesomeIconLinkRendererComponent } from 'src/app/shared/components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
import { PostingStatusEnum } from 'src/app/shared/models/enums/posting-status-enum';

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
  public postings = [];
  public tradesGridColumnDefs: ColDef[];
  public postingsGridColumnDefs: ColDef[];

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private tradeService: TradeService,
    private parcelService: ParcelService,
    private postingService: PostingService,
    private currencyPipe: CurrencyPipe,
    private datePipe: DatePipe) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;

      forkJoin(
        this.parcelService.getParcelsWithLandOwners(),
        this.tradeService.getAllTradeActivity(),
        this.postingService.getPostingsDetailed(),
      ).subscribe(([parcels, trades, postings]) => {
        this.parcels = parcels;
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
            headerName: 'Date', field: 'OfferDate', valueFormatter: function(params) {
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
          { headerName: 'Status', 
            valueGetter: function(params) { return params.data.TradeStatus.TradeStatusID === TradeStatusEnum.Accepted && (!params.data.IsRegisteredByBuyer || !params.data.IsRegisteredBySeller) ? "Awaiting Registration" : params.data.TradeStatus.TradeStatusDisplayName; }, 
            sortable: true, filter: true, width: 200 },
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
          { headerName: 'Quantity (acre-feet)', field: 'Quantity', sortable: true, filter: true, width: 160 },
          { headerName: 'Unit Price', field: 'Price', valueFormatter: function (params) { return _currencyPipe.transform(params.value, "USD"); }, sortable: true, filter: true, width: 100 },
          { headerName: 'Total Price', valueGetter: function(params) { return params.data.Price * params.data.Quantity; }, valueFormatter: function (params) { return _currencyPipe.transform(params.value, "USD"); }, sortable: true, filter: true, width: 130 },
          {
            headerName: 'Posted By', field: 'OfferCreateUser', cellRendererFramework: LinkRendererComponent,
            cellRendererParams: { inRouterLink: "/users/" },
            filterValueGetter: function (params: any) {
              return params.data.OfferCreateUser.FullName;
            },
            comparator: function (id1: any, id2: any) {
              let user1 = id1 ? id1.LastName + ", " + id1.FirstName : '';
              let user2 = id2 ? id2.LastName + ", " + id2.FirstName : '';
              if (user1 < user2) {
                return -1;
              }
              if (user1 > user2) {
                return 1;
              }
              return 0;
            },
            sortable: true, filter: true, width: 155
          },
        ];
        this.trades = trades;

        this.postingsGridColumnDefs = [
            { headerName: '', valueGetter: 
              function (params: any) {
                if(params.data.NumberOfOffers === 0 && params.data.PostingStatusID === PostingStatusEnum.Open)
                {
                  return params.data.PostingID;
                }
                return null;
              }
            , cellRendererFramework: FontAwesomeIconLinkRendererComponent, 
            cellRendererParams: { inRouterLink: "/delete-posting", fontawesomeIconName: 'trash' },
            sortable: false, filter: false, width: 30 },
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
            headerName: 'Posted By', 
            valueGetter: function (params: any) {
              return { UserID: params.data.PostedByUserID, FullName: params.data.PostedByFullName };
            }, cellRendererFramework: LinkRendererComponent,
            cellRendererParams: { inRouterLink: "/users/" },
            filterValueGetter: function (params: any) {
              return params.data.PostedByFullName;
            },
            comparator: function (id1: any, id2: any) {
              let user1 = id1 ? id1.PostedByLastName + ", " + id1.PostedByFirstName : '';
              let user2 = id2 ? id2.PostedByLastName + ", " + id2.PostedByFirstName : '';
              if (user1 < user2) {
                return -1;
              }
              if (user1 > user2) {
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
}
