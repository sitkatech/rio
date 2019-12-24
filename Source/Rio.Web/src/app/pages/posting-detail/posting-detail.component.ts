import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { PostingService } from 'src/app/services/posting.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { PostingDto } from 'src/app/shared/models/posting/posting-dto';
import { OfferUpsertDto } from 'src/app/shared/models/offer/offer-upsert-dto';
import { OfferService } from 'src/app/services/offer.service';
import { OfferDto } from 'src/app/shared/models/offer/offer-dto';
import { Alert } from 'src/app/shared/models/alert';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { OfferStatusEnum } from 'src/app/shared/models/enums/offer-status-enum';
import { PostingTypeDto } from 'src/app/shared/models/posting/posting-type-dto';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';
import { PostingUpdateStatusDto } from 'src/app/shared/models/posting/posting-update-status-dto';
import { PostingStatusEnum } from 'src/app/shared/models/enums/posting-status-enum';
import { UserDto } from 'src/app/shared/models';
import { AccountSimpleDto } from 'src/app/shared/models/account/account-simple-dto';

@Component({
    selector: 'template-posting-detail',
    templateUrl: './posting-detail.component.html',
    styleUrls: ['./posting-detail.component.scss']
})
export class PostingDetailComponent implements OnInit, OnDestroy {
    private watchUserChangeSubscription: any;
    private currentUser: UserDto;

    public posting: PostingDto;
    public offers: Array<OfferDto>;
    public model: OfferUpsertDto;
    public isLoadingSubmit: boolean = false;

    public isCounterOffering: boolean = false;
    public isConfirmingTrade: boolean = false;
    public isRejectingTrade: boolean = false;
    public isRescindingTrade: boolean = false;

    public originalPostingType: PostingTypeDto;
    public offerType: string;
    public counterOfferRecipientType: string;
    currentAccount: AccountSimpleDto;

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
        this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
            this.currentUser = currentUser;
            this.authenticationService.getActiveAccount().subscribe(account=>{
                this.currentAccount = account;
            })
            const postingID = parseInt(this.route.snapshot.paramMap.get("postingID"));
            if (postingID) {
                forkJoin(
                    this.postingService.getPostingFromPostingID(postingID),
                    this.offerService.getActiveOffersFromPostingIDForCurrentUser(postingID)
                ).subscribe(([posting, offers]) => {
                    this.posting = posting instanceof Array
                        ? null
                        : posting as PostingDto;

                    this.offers = offers.sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0);
                    this.resetModelToPosting();
                    this.originalPostingType = this.posting.PostingType;
                    this.offerType = this.isPostingOwner ?
                        (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToBuy ? "Purchasing" : "Selling")
                        : (this.originalPostingType.PostingTypeID === PostingTypeEnum.OfferToBuy ? "Selling" : "Purchasing");
                    this.counterOfferRecipientType = this.offerType === "Purchasing" ? "seller" : "buyer";
                });
            }
        });
    }

    ngOnDestroy() {
        this.watchUserChangeSubscription.unsubscribe();
        this.authenticationService.dispose();
        this.cdr.detach();
    }

    public isPostingOwner(): boolean{
        return this.posting.CreateAccount.AccountID === this.currentAccount.AccountID;
    }

    private resetModelToPosting() {
        let offer = new OfferUpsertDto();
        if (this.offers.length > 0) {
            // get most recent offer
            var mostRecentOffer = this.offers.sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0)[0];
            offer.TradeID = mostRecentOffer.TradeID;
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
                this.router.navigateByUrl("/landowner-dashboard").then(x => {
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
                this.isLoadingSubmit = false;
                this.router.navigateByUrl("/landowner-dashboard").then(x => {
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
