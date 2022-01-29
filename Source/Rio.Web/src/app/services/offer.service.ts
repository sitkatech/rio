import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { OfferDto } from '../shared/generated/model/offer-dto';
import { OfferUpsertDto } from '../shared/generated/model/offer-upsert-dto';

@Injectable({
    providedIn: 'root'
})
export class OfferService {

    constructor(private apiService: ApiService) { }

    getActiveOffersFromPostingIDForCurrentUser(postingID: number): Observable<OfferDto[]> {
        let route = `/current-user-active-offers/${postingID}`;
        return this.apiService.getFromApi(route);
    }

    getActiveOffersFromPostingIDForCurrentAccount(postingID: number, accountID: number): Observable<OfferDto[]> {
        let route = `/current-account-active-offers/${accountID}/${postingID}`;
        return this.apiService.getFromApi(route);
    }

    getOfferFromOfferID(offerID: number): Observable<OfferDto> {
        let route = `/offers/${offerID}`;
        return this.apiService.getFromApi(route);
    }

    getOffersFromTradeNumber(tradeNumber: string): Observable<OfferDto[]> {
        let route = `/trades/${tradeNumber}/offers`;
        return this.apiService.getFromApi(route);
    }

    newOffer(postingID: number, offerNewDto: OfferUpsertDto): Observable<OfferDto>  {
      let route = `/offers/new/${postingID}`;
      return this.apiService.postToApi(route, offerNewDto);
    }
}
