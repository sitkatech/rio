import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { TradeDto } from 'src/app/shared/models/offer/trade-dto';
import { OfferDto } from 'src/app/shared/models/offer/offer-dto';
import { OfferUpsertDto } from 'src/app/shared/models/offer/offer-upsert-dto';
import { ActivatedRoute, Router } from '@angular/router';
import { OfferService } from 'src/app/services/offer.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { forkJoin, Observable, Subject } from 'rxjs';
import { OfferStatusEnum } from 'src/app/shared/models/enums/offer-status-enum';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { TradeService } from 'src/app/services/trade.service';
import { Alert } from 'src/app/shared/models/alert';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';
import { PostingTypeDto } from 'src/app/shared/models/posting/posting-type-dto';
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';
import { UserDto } from 'src/app/shared/models';

@Component({
  selector: 'rio-trade-detail',
  templateUrl: './trade-detail.component.html',
  styleUrls: ['./trade-detail.component.scss']
})
export class TradeDetailComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public trade: TradeDto;
  public offers: Array<OfferDto>;
  public mostRecentOffer: OfferDto;
  public model: OfferUpsertDto;
  public isLoadingSubmit: boolean = false;

  public isCounterOffering: boolean = false;
  public isConfirmingTrade: boolean = false;
  public isRejectingTrade: boolean = false;
  public isRescindingTrade: boolean = false;

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
      this.resetModelToMostRecentOffer();
      let currentUserID = this.currentUser.UserID;
      this.isTradeOwner = this.trade.CreateUser.UserID === currentUserID;
      this.isPostingOwner = this.trade.Posting.CreateUser.UserID === currentUserID;
      this.isCurrentOfferCreator = this.mostRecentOffer.CreateUser.UserID === currentUserID;
      this.originalPostingType = this.trade.Posting.PostingType;
      this.offerType = this.isPostingOwner ?
        (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToBuy ? "Purchasing" : "Selling")
        : (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToBuy ? "Selling" : "Purchasing");
      this.counterOfferRecipientType = this.offerType === "Purchasing" ? "seller" : "buyer";

      // reset the action states to initial (false) state
      this.isCounterOffering = false;
      this.isConfirmingTrade = false;
      this.isRejectingTrade = false;
      this.isRescindingTrade = false;


    });
  }

  private resetModelToMostRecentOffer(): void {
    let offer = new OfferUpsertDto();
    offer.TradeID = this.trade.TradeID;
    offer.Price = this.mostRecentOffer.Price;
    offer.Quantity = this.mostRecentOffer.Quantity;
    offer.OfferStatusID = OfferStatusEnum.Pending;
    this.model = offer;
  }

  public isTradeNotOpen(): boolean {
    return this.trade.TradeStatus.TradeStatusID !== TradeStatusEnum.Open;
  }

  public getTotalPrice(offer: any): number {
    return offer.Price * offer.Quantity;
  }

  public getOfferType(offer: OfferDto): string {
    if (offer.OfferStatus.OfferStatusID === OfferStatusEnum.Pending) {
      if (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToSell
        && offer.CreateUser.UserID === this.trade.CreateUser.UserID) {
        return "Buyer Counter Offer";
      }
      else {
        return "Seller Counter Offer";
      }
    }
    else {
      return "Offer " + offer.OfferStatus.OfferStatusDisplayName;
    }
  }

  public rescindOffer(): void {
    this.isRescindingTrade = true;
    this.isConfirmingTrade = true;
    this.model.OfferStatusID = OfferStatusEnum.Rescinded;
  }

  public rejectOffer(): void {
    this.isRejectingTrade = true;
    this.isConfirmingTrade = true;
    this.model.OfferStatusID = OfferStatusEnum.Rejected;
  }

  public acceptCurrentOffer(): void {
    this.isConfirmingTrade = true;
    this.model.OfferStatusID = OfferStatusEnum.Accepted;
  }

  public cancelCounterOffer(): void {
    this.isCounterOffering = false;
    this.resetModelToMostRecentOffer();
  }

  public cancelConfirmation(): void {
    this.isConfirmingTrade = false;
    this.isRejectingTrade = false;
    this.isRescindingTrade = false;
    this.model.OfferStatusID = OfferStatusEnum.Pending;
  }

  public confirmTrade(): void {
    this.isLoadingSubmit = true;
    this.offerService.newOffer(this.trade.TradeID, this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        this.router.navigateByUrl("/landowner-dashboard")
          .then(() => {
            this.getData(this.trade.TradeID);
            this.alertService.pushAlert(new Alert("Your request was successfully submitted.", AlertContext.Success));
          });
      }
        ,
        error => {
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }
}
