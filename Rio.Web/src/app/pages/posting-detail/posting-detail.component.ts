import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { PostingService } from 'src/app/services/posting.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { OfferService } from 'src/app/services/offer.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { OfferStatusEnum } from 'src/app/shared/generated/enum/offer-status-enum';
import { PostingTypeEnum } from 'src/app/shared/generated/enum/posting-type-enum';
import { PostingStatusEnum } from 'src/app/shared/generated/enum/posting-status-enum';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { OfferDto } from 'src/app/shared/generated/model/offer-dto';
import { OfferUpsertDto } from 'src/app/shared/generated/model/offer-upsert-dto';
import { PostingDto } from 'src/app/shared/generated/model/posting-dto';
import { PostingTypeDto } from 'src/app/shared/generated/model/posting-type-dto';
import { PostingUpdateStatusDto } from 'src/app/shared/generated/model/posting-update-status-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';

@Component({
    selector: 'template-posting-detail',
    templateUrl: './posting-detail.component.html',
    styleUrls: ['./posting-detail.component.scss']
})
export class PostingDetailComponent implements OnInit, OnDestroy {
    
    private currentUser: UserDto;

    public posting: PostingDto;
    public offers: Array<OfferDto>;
    public model: OfferUpsertDto;
    public successfulOfferDto: OfferDto;
    public isLoadingSubmit: boolean = false;

    public isCounterOffering: boolean = false;
    public isConfirmingTrade: boolean = false;
    public isRejectingTrade: boolean = false;
    public isRescindingTrade: boolean = false;
    public offerSubmittedSuccessfully: boolean = false;

    public originalPostingType: PostingTypeDto;
    public offerType: string;
    public counterOfferRecipientType: string;
    public currentAccount: AccountSimpleDto;
    public currentUserAccounts: AccountSimpleDto[];
    public currentAccountChosen: boolean =  false;
    confirmedTrade: boolean;

    constructor(
        private cdr: ChangeDetectorRef,
        private route: ActivatedRoute,
        private router: Router,
        private postingService: PostingService,
        private offerService: OfferService,
        private authenticationService: AuthenticationService,
        private alertService: AlertService
    ) {
    }

    ngOnInit() {
        this.authenticationService.getCurrentUser().subscribe(currentUser => {
            this.currentUser = currentUser;
            this.currentUserAccounts = this.authenticationService.getAvailableAccounts();
            const postingID = parseInt(this.route.snapshot.paramMap.get("postingID"));
            if (postingID) {
                this.postingService.getPostingFromPostingID(postingID).subscribe(posting => {
                    this.posting = posting instanceof Array
                        ? null
                        : posting as PostingDto;
                    this.originalPostingType = this.posting.PostingType;
                    if (this.currentUserAccounts?.length == 1) {
                        this.currentAccount = this.currentUserAccounts[0];
                        this.getOffersForCurrentAccount();
                    }
                });
            }
        });
    }

    ngOnDestroy() {
        
        
        this.cdr.detach();
    }

    public getOffersForCurrentAccount() {
        if (this.currentAccount) {
            this.currentAccountChosen = true;
            this.offerService.getActiveOffersFromPostingIDForCurrentAccount(this.posting.PostingID, this.currentAccount.AccountID).subscribe(offers => {
                this.offers = offers.sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0);
                this.offerType = this.isPostingOwner() ?
                    (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToBuy ? "Purchasing" : "Selling")
                    : (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToBuy ? "Selling" : "Purchasing");
                this.counterOfferRecipientType = this.offerType === "Purchasing" ? "seller" : "buyer";
                this.cdr.detectChanges();
                this.resetModelToPosting();
            });
        }
    }

    public isPostingOwner(): boolean {
        return (this.currentAccount) && this.posting.CreateAccount.AccountID === this.currentAccount.AccountID;
    }

    private resetModelToPosting() {
        if (!this.offers) {
            throw new Error("Offers not intialized!");
        }
        if (!this.posting) {
            throw new Error("Posting not initialized!");
        }

        let offer = new OfferUpsertDto();
        if (this.offers.length > 0) {
            // get most recent offer
            var mostRecentOffer = this.offers.sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0)[0];
            offer.TradeID = mostRecentOffer.Trade?.TradeID;
            offer.OfferID = mostRecentOffer.OfferID;
            offer.Price = mostRecentOffer.Price;
            offer.Quantity = mostRecentOffer.Quantity;
            offer.OfferNotes = mostRecentOffer.OfferNotes;
        }
        else {
            // use the posting as the initial offer
            offer.Price = this.posting.Price;
            offer.Quantity = this.posting.AvailableQuantity;
        }
        offer.OfferStatusID = OfferStatusEnum.Pending;
        this.model = offer;
        this.offerSubmittedSuccessfully = false;
    }

    public canEditCurrentPosting(): boolean {
        return this.isPostingOpen() && (this.authenticationService.isUserAnAdministrator(this.currentUser) || this.isPostingOwner());
    }

    private isPostingOpen() {
        return this.posting.PostingStatus.PostingStatusID === PostingStatusEnum.Open;
    }

    public getTotalPrice(posting: any): number {
        return posting.Price * posting.Quantity;
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
        this.resetModelToPosting();
    }

    public cancelConfirmation(): void {
        this.isConfirmingTrade = false;
        this.isRejectingTrade = false;
        this.isRescindingTrade = false;
        this.model.OfferStatusID = OfferStatusEnum.Pending;
    }

    public closePosting(): void {
        this.isLoadingSubmit = true;
        var postingUpdateStatusDto = new PostingUpdateStatusDto();
        postingUpdateStatusDto.PostingStatusID = PostingStatusEnum.Closed;
        this.postingService.closePosting(this.posting.PostingID, postingUpdateStatusDto)
            .subscribe(response => {
                this.isLoadingSubmit = false;
                this.posting = response;
                this.alertService.pushAlert(new Alert("Your request was successfully submitted.", AlertContext.Success));
            }
                ,
                error => {
                    this.isLoadingSubmit = false;
                    this.cdr.detectChanges();
                }
            );
    }

    public isOfferFormValid(): boolean {
        return this.model.Price > 0 && this.model.Quantity > 0;
    }

    public isPendingOffer(): boolean {
        return this.model.OfferID > 0;
    }

    public confirmTrade(): void {
        this.isLoadingSubmit = true;
        this.model.CreateAccountID = this.currentAccount.AccountID;
        this.offerService.newOffer(this.posting.PostingID, this.model)
            .subscribe(response => {
                this.isConfirmingTrade = false;
                this.isCounterOffering = false;
                this.isLoadingSubmit = false;
                this.offerSubmittedSuccessfully = true;
                this.successfulOfferDto = response;
                this.alertService.pushAlert(new Alert("Your request was successfully submitted.", AlertContext.Success));
            }
                ,
                error => {
                    this.isLoadingSubmit = false;
                    this.cdr.detectChanges();
                }
            );
    }

    public currentAccountSet(): boolean {
        return this.currentAccount !== null && this.currentAccount !== undefined && this.currentAccountChosen;
    }

    public resetCurrentAccount() {
        this.currentAccount = undefined;
        this.currentAccountChosen = false;
    }

    public offersSet(): boolean {
        return this.offers !== null && this.offers !== undefined;
    }

    public offerAccepted(): boolean {
        return this.model.OfferStatusID == OfferStatusEnum.Accepted;
    }

    public landownerHasAnyAccounts(): boolean {
        return this.currentUserAccounts?.length > 0;
    }

    public userHasMoreThanOneAccount(): boolean {
        return this.currentUserAccounts?.length > 1;
    }
}
