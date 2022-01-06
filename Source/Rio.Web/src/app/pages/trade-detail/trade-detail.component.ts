import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
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
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';
import { WaterTransferService } from 'src/app/services/water-transfer.service';
import { WaterTransferTypeEnum } from 'src/app/shared/models/enums/water-transfer-type-enum';
import { WaterTransferRegistrationStatusEnum } from 'src/app/shared/models/enums/water-transfer-registration-status-enum';
import { AccountDto } from 'src/app/shared/generated/model/account-dto';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { OfferDto } from 'src/app/shared/generated/model/offer-dto';
import { OfferUpsertDto } from 'src/app/shared/generated/model/offer-upsert-dto';
import { PostingTypeDto } from 'src/app/shared/generated/model/posting-type-dto';
import { TradeDto } from 'src/app/shared/generated/model/trade-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTransferRegistrationSimpleDto } from 'src/app/shared/generated/model/water-transfer-registration-simple-dto';

@Component({
  selector: 'rio-trade-detail',
  templateUrl: './trade-detail.component.html',
  styleUrls: ['./trade-detail.component.scss']
})
export class TradeDetailComponent implements OnInit, OnDestroy {
  
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

  public isCurrentOfferCreator: boolean = false;

  public originalPostingType: PostingTypeDto;
  public offerType: string;
  public counterOfferRecipientType: string;
  public waterTransferRegistrations: Array<WaterTransferRegistrationSimpleDto>;
  public buyer: AccountDto;
  public seller: AccountDto;
  activeAccount: AccountSimpleDto;
  public tradeActionConfirmed: boolean = false;
  currentUserAccounts: AccountSimpleDto[];

  constructor(
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private offerService: OfferService,
    private tradeService: TradeService,
    private waterTransferService: WaterTransferService,
    private authenticationService: AuthenticationService,
    private alertService: AlertService
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
      this.currentUserAccounts = this.authenticationService.getAvailableAccounts();
      const tradeNumber = this.route.snapshot.paramMap.get("tradeNumber");
      const accountID = parseInt(this.route.snapshot.paramMap.get("accountID"));

      if (!tradeNumber || (!accountID && !this.authenticationService.isCurrentUserAnAdministrator()) || (accountID && this.currentUserAccounts?.filter(x => x.AccountID == accountID).length == 0)) {
        this.router.navigate(["/"]).then(() => {
          this.alertService.pushNotFoundUnauthorizedAlert();
        });
      }

      if (accountID) {
        this.activeAccount = this.currentUserAccounts.filter(x => x.AccountID == accountID)[0];
      }

      this.getData(tradeNumber);
    });
  }

  isPostingOwner(): boolean {
    return this.activeAccount && this.trade.Posting.CreateAccount.AccountID === this.activeAccount.AccountID;
  }
  isTradeOwner(): boolean {
    return this.activeAccount && this.trade.CreateAccount.AccountID === this.activeAccount.AccountID;
  }

  ngOnDestroy() {
    
    
    this.cdr.detach();
  }

  private getData(tradeNumber: string): void {
    forkJoin(
      this.tradeService.getTradeFromTradeNumber(tradeNumber),
      this.offerService.getOffersFromTradeNumber(tradeNumber)
    ).subscribe(([trade, offers]) => {
      this.trade = trade instanceof Array
        ? null
        : trade as TradeDto;
      this.offers = offers.sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0);
      this.mostRecentOffer = this.offers[0];
      this.resetModelToMostRecentOffer();
      let currentUserID = this.currentUser.UserID;
      this.isCurrentOfferCreator = this.activeAccount && this.mostRecentOffer.CreateAccount.AccountID === this.activeAccount.AccountID;
      this.originalPostingType = this.trade.Posting.PostingType;
      this.offerType = this.isPostingOwner ?
        (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToBuy ? "Purchasing" : "Selling")
        : (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToBuy ? "Selling" : "Purchasing");
      this.counterOfferRecipientType = this.offerType === "Purchasing" ? "seller" : "buyer";

      this.buyer = this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToBuy ? this.trade.Posting.CreateAccount : this.trade.CreateAccount;
      this.seller = this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToSell ? this.trade.Posting.CreateAccount : this.trade.CreateAccount;

      if (this.activeAccount.AccountID != this.buyer.AccountID && this.activeAccount.AccountID != this.seller.AccountID) {
        this.router.navigate(["/"]).then(() => {
          this.alertService.pushNotFoundUnauthorizedAlert();
        });
      }

      // reset the action states to initial (false) state
      this.isCounterOffering = false;
      this.isConfirmingTrade = false;
      this.isRejectingTrade = false;
      this.isRescindingTrade = false;

      this.waterTransferRegistrations = [];
      if (this.mostRecentOffer.WaterTransferID) {
        this.waterTransferService.getWaterTransferRegistrationsFromWaterTransferID(this.mostRecentOffer.WaterTransferID)
          .subscribe(result => {
            this.waterTransferRegistrations = result.filter(x => x.WaterTransferRegistrationStatusID !== WaterTransferRegistrationStatusEnum.Pending).sort((a, b) => a.StatusDate > b.StatusDate ? -1 : a.StatusDate < b.StatusDate ? 1 : 0);
          });
      }
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

  public getWaterTransferRegistrationType(waterTransferTypeID: number): string {
    return waterTransferTypeID === WaterTransferTypeEnum.Buying ? "Buyer" : "Seller";
  }

  public getWaterTransferRegistrationStatus(waterTransferRegistrationStatusID: number): string {
    switch (waterTransferRegistrationStatusID) {
      case WaterTransferRegistrationStatusEnum.Registered:
        return "Registered";
      case WaterTransferRegistrationStatusEnum.Canceled:
        return "Canceled";
      default:
        return "Pending"
    }
  }

  public getTradeStatus(trade: TradeDto): string {
    if (this.isCanceled()) {
      return "Transaction Canceled";
    }
    return "Offer " + trade.TradeStatus.TradeStatusDisplayName;
  }

  public canConfirmTransfer(): boolean {
    return !this.isCanceled() && this.mostRecentOffer.OfferStatus.OfferStatusID === OfferStatusEnum.Accepted
      && ((this.activeAccount.AccountID === this.buyer.AccountID && !this.isRegistered(WaterTransferTypeEnum.Buying)) ||
        (this.activeAccount.AccountID === this.seller.AccountID && !this.isRegistered(WaterTransferTypeEnum.Selling)));
  }

  private isCanceled() {
    if (this.waterTransferRegistrations && this.waterTransferRegistrations.length > 0) {
      return this.waterTransferRegistrations.filter(x => x.IsCanceled).length > 0;
    }
    return false;
  }

  private isRegistered(waterTransferType: WaterTransferTypeEnum) {
    if (this.waterTransferRegistrations.length > 0) {
      return this.waterTransferRegistrations.filter(x => x.WaterTransferTypeID === waterTransferType && x.IsRegistered).length > 0;
    }
    return false;
  }

  public isTradeNotOpen(): boolean {
    return this.trade.TradeStatus.TradeStatusID !== TradeStatusEnum.Countered;
  }

  public isOfferAccepted(offer: OfferDto): boolean {
    return offer.OfferStatus.OfferStatusID === OfferStatusEnum.Accepted;
  }

  public getTotalPrice(offer: any): number {
    return offer.Price * offer.Quantity;
  }

  public getOfferType(offer: OfferDto): string {
    if (offer.OfferStatus.OfferStatusID === OfferStatusEnum.Pending && this.originalPostingType) {
      if (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToSell) {
        return offer.CreateAccount.AccountID === this.trade.CreateAccount.AccountID ? "Buyer Counter Offer" : "Seller Counter Offer";
      }
      else {
        return offer.CreateAccount.AccountID === this.trade.CreateAccount.AccountID ? "Seller Counter Offer" : "Buyer Counter Offer";
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
    this.model.CreateAccountID = this.activeAccount.AccountID;
    this.offerService.newOffer(this.trade.Posting.PostingID, this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        this.isConfirmingTrade = false;
        this.alertService.pushAlert(new Alert(this.getAlertMessage(), AlertContext.Success));
        this.getData(this.route.snapshot.paramMap.get("tradeNumber"));
      },
        error => {
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }

  public getAlertMessage(): string {
    switch (this.model.OfferStatusID) {
      case OfferStatusEnum.Accepted:
        return "Offer successfully accepted.";
      case OfferStatusEnum.Rejected:
        return "Offer successfully rejected.";
      case OfferStatusEnum.Rescinded:
        return "Offer successfully rescinded.";
      case OfferStatusEnum.Pending:
        if (this.isCounterOffering) {
          return "Counter offer successfully submitted.";
        }
    }
  }

  public isOfferFormValid(): boolean {
    return this.model.Price > 0 && this.model.Quantity > 0;
  }
}
