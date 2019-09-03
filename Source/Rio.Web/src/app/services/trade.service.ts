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

    getAllTradeActivity(): Observable<TradeWithMostRecentOfferDto[]> {
        let route = `/all-trade-activity`;
        return this.apiService.getFromApi(route);
    }

    getTradeActivityForUser(userID: number): Observable<TradeWithMostRecentOfferDto[]> {
        let route = `/trade-activity/${userID}`;
        return this.apiService.getFromApi(route);
    }

    getTradeFromTradeID(tradeID: number): Observable<TradeDto> {
        let route = `/trades/${tradeID}`;
        return this.apiService.getFromApi(route);
    }
}
