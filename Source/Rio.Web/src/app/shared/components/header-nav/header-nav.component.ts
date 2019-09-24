import { Component, OnInit, HostListener, ChangeDetectorRef, ChangeDetectionStrategy, OnDestroy } from '@angular/core';
import { CookieStorageService } from '../../services/cookies/cookie-storage.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from '../../models';
import { TradeWithMostRecentOfferDto } from '../../models/offer/trade-with-most-recent-offer-dto';
import { TradeService } from 'src/app/services/trade.service';
import { forkJoin } from 'rxjs';
import { OfferStatusEnum } from '../../models/enums/offer-status-enum';
import { PostingTypeEnum } from '../../models/enums/posting-type-enum';

@Component({
    selector: 'header-nav',
    templateUrl: './header-nav.component.html',
    styleUrls: ['./header-nav.component.scss']
})

export class HeaderNavComponent implements OnInit, OnDestroy {
    private watchUserChangeSubscription: any;
    private currentUser: UserDto;
    public trades: Array<TradeWithMostRecentOfferDto>;

    windowWidth: number;

    @HostListener('window:resize', ['$event'])
    resize(ev?: Event) {
        this.windowWidth = window.innerWidth;
    }

    constructor(
        private authenticationService: AuthenticationService,
        private cookieStorageService: CookieStorageService,
        private tradeService: TradeService,
        private cdr: ChangeDetectorRef) {
    }

    ngOnInit() {
        this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
            this.currentUser = currentUser;
            if (currentUser) {
                forkJoin(
                    this.tradeService.getTradeActivityForUser(currentUser.UserID),
                ).subscribe(([trades]) => {
                    this.trades = trades ? trades.sort((a, b) => a.OfferDate > b.OfferDate ? -1 : a.OfferDate < b.OfferDate ? 1 : 0) : [];
                });
            }
        });
    }

    ngOnDestroy() {
        this.watchUserChangeSubscription.unsubscribe();
        this.authenticationService.dispose();
        this.cdr.detach();
    }

    public isAuthenticated(): boolean {
        return this.authenticationService.isAuthenticated();
    }

    public isLandOwner(): boolean {
        return this.authenticationService.isUserALandOwner(this.currentUser);
    }

    public isAdministrator(): boolean {
        return this.authenticationService.isUserAnAdministrator(this.currentUser);
    }

    public getUserName() {
        return this.currentUser ? this.currentUser.FullName
            : null;
    }

    public login(): void {
        this.authenticationService.login();
    }

    public logout(): void {
        this.authenticationService.logout();

        setTimeout(() => {
            this.cookieStorageService.removeAll();
            this.cdr.detectChanges();
        });
    }

    public getPendingTrades(): Array<TradeWithMostRecentOfferDto> {
        const pendingTrades = this.trades !== undefined ? this.trades.filter(tr => this.isTradePending(tr)
            || (tr.OfferStatus.OfferStatusID === OfferStatusEnum.Accepted && !this.isTradeRegisteredByUser(tr))) : [];
        return pendingTrades;
    }

    public doesMostRecentOfferBelongToCurrentUser(trade: TradeWithMostRecentOfferDto): boolean {
        return trade.OfferCreateUser.UserID === this.currentUser.UserID;
    }

    public getOfferThatBelongsToYouNotificationText(trade: TradeWithMostRecentOfferDto): string {
        let offerType = trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "sell" : "purchase";
        let respondee = trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "buyer" : "seller";
        return "You have submitted an offer to " + offerType + " " + trade.Quantity + " ac-ft of water. The " + respondee + " has " + this.getDaysLeftToRespond(trade) + " days to respond.";
    }

    public getOfferThatDoesNotBelongToYouNotificationText(trade: TradeWithMostRecentOfferDto): string {
        if (trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell) {
            return "A seller is awaiting your response to their offer to sell " + trade.Quantity + " ac-ft of water. This offer expires in " + this.getDaysLeftToRespond(trade) + " days.";
        }
        return "A buyer is awaiting your response to their offer to purchase " + trade.Quantity + " ac-ft of water. This offer expires in " + this.getDaysLeftToRespond(trade) + " days.";
    }

    public isTradeRegisteredByUser(trade: TradeWithMostRecentOfferDto) {
        return (trade.IsConfirmedByBuyer && trade.Buyer.UserID === this.currentUser.UserID) || (trade.IsConfirmedBySeller && trade.Seller.UserID === this.currentUser.UserID);
    }

    public isTradePending(trade: TradeWithMostRecentOfferDto) {
        return trade.OfferStatus.OfferStatusID === OfferStatusEnum.Pending;
    }

    public getDaysLeftToRespond(trade: TradeWithMostRecentOfferDto): number {
        //TODO: get logic to calculated days left to respond; hardcoded to 5 for now
        return 5;
    }
}
