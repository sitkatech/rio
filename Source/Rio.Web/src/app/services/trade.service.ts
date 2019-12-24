import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { TradeDto } from '../shared/models/offer/trade-dto';
import { TradeWithMostRecentOfferDto } from '../shared/models/offer/trade-with-most-recent-offer-dto';

@Injectable({
    providedIn: 'root'
})
export class TradeService {

    constructor(private apiService: ApiService) { }

    getTradeActivityForYear(year: number): Observable<TradeWithMostRecentOfferDto[]> {
        let route = `/all-trade-activity/${year}`;
        return this.apiService.getFromApi(route);
    }

    getTradeActivityByAccountID(accountID: number): Observable<TradeWithMostRecentOfferDto[]> {
        let route = `/trade-activity/${accountID}`;
        return this.apiService.getFromApi(route);
    }

    getTradeFromTradeNumber(tradeNumber: string): Observable<TradeDto> {
        let route = `/trades/${tradeNumber}`;
        return this.apiService.getFromApi(route);
    }
}
