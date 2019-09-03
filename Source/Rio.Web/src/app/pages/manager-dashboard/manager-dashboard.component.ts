import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { UserDto } from 'src/app/shared/models';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { TradeWithMostRecentOfferDto } from 'src/app/shared/models/offer/trade-with-most-recent-offer-dto';
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';
import { TradeService } from 'src/app/services/trade.service';
import { forkJoin } from 'rxjs';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';
import { OfferStatusEnum } from 'src/app/shared/models/enums/offer-status-enum';

@Component({
  selector: 'rio-manager-dashboard',
  templateUrl: './manager-dashboard.component.html',
  styleUrls: ['./manager-dashboard.component.scss']
})
export class ManagerDashboardComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public parcels: Array<ParcelDto>;

  public waterYearToDisplay: number;
  private tradeStatusIDs: TradeStatusEnum[];
  public trades: Array<TradeWithMostRecentOfferDto>;
  public waterYears: Array<number>;


  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private tradeService: TradeService,
    private parcelService: ParcelService) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.waterYears = [2018, 2017, 2016]; //TODO: get this from API
      this.waterYearToDisplay = 2018; //TODO: get this from API
      this.tradeStatusIDs = [TradeStatusEnum.Open];

      forkJoin(
        this.parcelService.getParcelsWithLandOwners(),
        this.tradeService.getTradeActivityForUser(this.currentUser.UserID),
      ).subscribe(([parcels, trades]) => {
        this.parcels = parcels;        
        this.trades = trades;        
      });


      this.parcelService.getParcelsWithLandOwners().subscribe(result => {
        this.parcels = result;
        this.cdr.detectChanges();
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

  public toggleTradeStatusShown(): void {
    if (this.tradeStatusIDs.length === 1) {
      this.tradeStatusIDs = [TradeStatusEnum.Accepted, TradeStatusEnum.Open, TradeStatusEnum.Rejected, TradeStatusEnum.Rescinded];
    }
    else {
      this.tradeStatusIDs = [TradeStatusEnum.Open];
    }
  }

  public getNoTradesMessage(): string {
    if (this.tradeStatusIDs.length === 1) {
      return "There are no open trades.";
    }
    else {
      return "There is no trade activity.";
    }
  }

  public getTradesForWaterYear(): Array<TradeWithMostRecentOfferDto> {
    return this.trades
    .filter(x => (new Date(x.OfferDate).getFullYear() - 1).toString() === this.waterYearToDisplay.toString() && (this.tradeStatusIDs.includes(x.TradeStatus.TradeStatusID) || (x.OfferStatus.OfferStatusID === OfferStatusEnum.Accepted && !x.IsConfirmed)))
    .sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0);
  }

  public doesMostRecentOfferBelongToCurrentUser(trade: TradeWithMostRecentOfferDto): boolean {
    return trade.OfferCreateUser.UserID === this.currentUser.UserID;
  }

  public getTradeStatus(trade: TradeWithMostRecentOfferDto): string {
    return (this.doesMostRecentOfferBelongToCurrentUser(trade) ? "You " : "They ") + trade.TradeStatus.TradeStatusDisplayName.toLowerCase();
  }

  public getTradeDescription(trade: TradeWithMostRecentOfferDto): string {
    return (this.doesMostRecentOfferBelongToCurrentUser(trade) ? trade.OfferPostingTypeID === PostingTypeEnum.OfferToBuy ? "Buying " : "Selling " : trade.OfferPostingTypeID === PostingTypeEnum.OfferToBuy ? "Selling " : "Buying ") + " " + trade.Quantity + " ac-ft";
  }

}
