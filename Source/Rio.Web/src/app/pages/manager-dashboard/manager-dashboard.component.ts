import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { UserDto } from 'src/app/shared/models';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { TradeWithMostRecentOfferDto } from 'src/app/shared/models/offer/trade-with-most-recent-offer-dto';
import { TradeService } from 'src/app/services/trade.service';
import { forkJoin } from 'rxjs';
import { ColDef } from 'ag-grid-community';
import { TradeDateLinkRendererComponent } from 'src/app/shared/components/ag-grid/trade-date-link-renderer/trade-date-link-renderer.component';

@Component({
  selector: 'rio-manager-dashboard',
  templateUrl: './manager-dashboard.component.html',
  styleUrls: ['./manager-dashboard.component.scss']
})
export class ManagerDashboardComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  public currentUser: UserDto;

  public parcels: Array<ParcelDto>;

  public rowData = [];
  columnDefs: ColDef[];

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private tradeService: TradeService,
    private parcelService: ParcelService) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;

      forkJoin(
        this.parcelService.getParcelsWithLandOwners(),
        this.tradeService.getTradeActivityForUser(this.currentUser.UserID),
      ).subscribe(([parcels, trades]) => {
        this.parcels = parcels;        
        
        this.columnDefs = [
          {
            headerName: 'Date', valueGetter: function (params: any) {
              return { OfferDate: params.data.OfferDate, TradeID: params.data.TradeID };
            }, cellRendererFramework: TradeDateLinkRendererComponent,
            cellRendererParams: { inRouterLink: "/trades/" },
            filterValueGetter: function (params: any) {
              return params.data.OfferDate;
            },
            comparator: function (id1: any, id2: any) {
              if (id1.OfferDate < id2.OfferDate) {
                return -1;
              }
              if (id1.OfferDate > id2.OfferDate) {
                return 1;
              }
              return 0;
            },
            sortable: true, filter: true
          },
          { headerName: 'Posted By', field: 'OfferCreateUser.FullName', sortable: true, filter: true },
          { headerName: 'Price', field: 'Price', sortable: true, filter: true },
        ];  
        this.rowData = trades;
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
