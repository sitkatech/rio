import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { TradeDto } from 'src/app/shared/models/offer/trade-dto';
import { OfferDto } from 'src/app/shared/models/offer/offer-dto';
import { PostingTypeDto } from 'src/app/shared/models/posting/posting-type-dto';
import { ActivatedRoute, Router } from '@angular/router';
import { OfferService } from 'src/app/services/offer.service';
import { TradeService } from 'src/app/services/trade.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { forkJoin } from 'rxjs';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';
import { OfferStatusEnum } from 'src/app/shared/models/enums/offer-status-enum';
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';

@Component({
  selector: 'rio-confirm-transfer',
  templateUrl: './confirm-transfer.component.html',
  styleUrls: ['./confirm-transfer.component.scss']
})
export class ConfirmTransferComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public trade: TradeDto;
  public offers: Array<OfferDto>;
  public mostRecentOffer: OfferDto;

  public isPostingOwner: boolean = false;
  public isTradeOwner: boolean = false;
  public isCurrentOfferCreator: boolean = false;

  public originalPostingType: PostingTypeDto;
  public offerType: string;
  public counterOfferRecipientType: string;

  constructor(
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private offerService: OfferService,
    private tradeService: TradeService,
    private authenticationService: AuthenticationService,
    private alertService: AlertService
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      const tradeID = parseInt(this.route.snapshot.paramMap.get("tradeID"));
      if (tradeID) {
        this.getData(tradeID);
      }
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  private getData(tradeID: number): void {
    forkJoin(this.tradeService.getTradeFromTradeID(tradeID), this.offerService.getOffersFromTradeID(tradeID)).subscribe(([trade, offers]) => {
      this.trade = trade instanceof Array
        ? null
        : trade as TradeDto;
      this.offers = offers.sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0);
      this.mostRecentOffer = this.offers[0];
      let currentUserID = this.currentUser.UserID;
      this.isTradeOwner = this.trade.CreateUser.UserID === currentUserID;
      this.isPostingOwner = this.trade.Posting.CreateUser.UserID === currentUserID;
      this.isCurrentOfferCreator = this.mostRecentOffer.CreateUser.UserID === currentUserID;
      this.originalPostingType = this.trade.Posting.PostingType;
      this.offerType = this.isPostingOwner ?
        (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToBuy ? "Purchasing" : "Selling")
        : (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToBuy ? "Selling" : "Purchasing");
      this.counterOfferRecipientType = this.offerType === "Purchasing" ? "seller" : "buyer";
    });
  }

  public isTradeNotOpen(): boolean {
    return this.trade.TradeStatus.TradeStatusID !== TradeStatusEnum.Open;
  }

  public getTotalPrice(offer: any): number {
    return offer.Price * offer.Quantity;
  }

  public getOfferType(offer: OfferDto): string {
    if (offer.OfferStatus.OfferStatusID === OfferStatusEnum.Pending) {
      if (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToSell) {
        return offer.CreateUser.UserID === this.trade.CreateUser.UserID ? "Buyer Counter Offer" : "Seller Counter Offer";
      }
      else {
        return offer.CreateUser.UserID === this.trade.CreateUser.UserID ? "Seller Counter Offer" : "Buyer Counter Offer";
      }
    }
    else {
      return "Offer " + offer.OfferStatus.OfferStatusDisplayName;
    }
  }
}
