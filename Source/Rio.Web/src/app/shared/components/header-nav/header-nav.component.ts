import { Component, OnInit, HostListener, ChangeDetectorRef, ChangeDetectionStrategy, OnDestroy } from '@angular/core';
import { CookieStorageService } from '../../services/cookies/cookie-storage.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from '../../models';
import { TradeWithMostRecentOfferDto } from '../../models/offer/trade-with-most-recent-offer-dto';
import { TradeService } from 'src/app/services/trade.service';
import { forkJoin } from 'rxjs';
import { OfferStatusEnum } from '../../models/enums/offer-status-enum';
import { PostingTypeEnum } from '../../models/enums/posting-type-enum';
import { AccountSimpleDto } from '../../models/account/account-simple-dto';

@Component({
    selector: 'header-nav',
    templateUrl: './header-nav.component.html',
    styleUrls: ['./header-nav.component.scss']
})

export class HeaderNavComponent implements OnInit, OnDestroy {
    private watchUserChangeSubscription: any;
    private currentUser: UserDto;
    public activeAccount: AccountSimpleDto;
    public trades: Array<TradeWithMostRecentOfferDto>;

    windowWidth: number;

    public accountDropdownConfig = {
      search: true,
      height: '320px',
      placeholder: "",
      displayKey: "AccountDisplayName",
      searchOnKey: "AccountDisplayName",
    }

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
            // display the correct active account in the dropdown below the username.
            // on pages which need to react to the active account, include this call in ngInit and put reactice logic in the subscribe statement.
            this.authenticationService.getActiveAccount().subscribe((account: AccountSimpleDto) => { this.activeAccount = account; });

            if (currentUser) {
                forkJoin(
                    this.tradeService.getTradeActivityByAccountID(currentUser.UserID),
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
            || (tr.OfferStatus.OfferStatusID === OfferStatusEnum.Accepted && this.isTradePendingRegistrationForAccount(tr))) : [];
        return pendingTrades;
    }

    public doesMostRecentOfferBelongToCurrentAccount(trade: TradeWithMostRecentOfferDto): boolean {
        return trade.OfferCreateAccount.AccountID === this.activeAccount.AccountID;
    }

    public getOfferThatBelongsToYouNotificationText(trade: TradeWithMostRecentOfferDto): string {
        let offerType = trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "sell" : "purchase";
        let respondee = trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell ? "buyer" : "seller";
        return "Trade " + trade.TradeNumber + " - You have submitted an offer to " + offerType + " water. The " + respondee + " has " + this.getDaysLeftToRespond(trade) + " days to respond.";
    }

    public getOfferThatDoesNotBelongToYouNotificationText(trade: TradeWithMostRecentOfferDto): string {
        if (trade.OfferPostingTypeID === PostingTypeEnum.OfferToSell) {
            return "Trade " + trade.TradeNumber + " - A seller is awaiting your response to their offer to sell water. This offer expires in " + this.getDaysLeftToRespond(trade) + " days.";
        }
        return "Trade " + trade.TradeNumber + " - A buyer is awaiting your response to their offer to purchase water. This offer expires in " + this.getDaysLeftToRespond(trade) + " days.";
    }

    public isTradePendingRegistrationForAccount(trade: TradeWithMostRecentOfferDto) {
        if(!trade.BuyerRegistration && !trade.SellerRegistration)
        {
            return false;
        }
        let isCanceled =  trade.BuyerRegistration.IsCanceled || trade.SellerRegistration.IsCanceled;
        return !isCanceled && ((trade.BuyerRegistration.IsPending && trade.Buyer.AccountID === this.activeAccount.AccountID) 
        || (trade.SellerRegistration.IsPending && trade.Seller.AccountID === this.activeAccount.AccountID));
    }

    public isTradePending(trade: TradeWithMostRecentOfferDto) {
        return trade.OfferStatus.OfferStatusID === OfferStatusEnum.Pending;
    }

    public getDaysLeftToRespond(trade: TradeWithMostRecentOfferDto): number {
        //TODO: get logic to calculated days left to respond; hardcoded to 5 for now
        return 5;
    }

    public getAvailableAccounts(): Array<AccountSimpleDto>{
        return this.authenticationService.getAvailableAccounts();
    }

    public setCurrentAccount(): void{
        this.authenticationService.setActiveAccount(this.activeAccount);
    }

    public compareAccountsFn(c1: AccountSimpleDto, c2: AccountSimpleDto): boolean {
        return c1 && c2 ? c1.AccountID === c2.AccountID : c1 === c2;
    }
}
