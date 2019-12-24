import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { PostingDto } from '../shared/models/posting/posting-dto';
import { TradeDto } from '../shared/models/offer/trade-dto';

@Injectable({
    providedIn: 'root'
})
export class PostingService {

    constructor(private apiService: ApiService) { }

    getPostings(): Observable<PostingDto[]> {
        let route = `/postings`;
        return this.apiService.getFromApi(route);
    }

    getPostingsByAccountID(accountID: number): Observable<PostingDto[]> {
        let route = `/accounts/${accountID}/postings`;
        return this.apiService.getFromApi(route);
    }

    getPostingFromPostingID(postingID: number): Observable<PostingDto> {
        let route = `/postings/${postingID}`;
        return this.apiService.getFromApi(route);
    }

    newPosting(postingNewDto: any): Observable<PostingDto>  {
      let route = `/postings/new`;
      return this.apiService.postToApi(route, postingNewDto);
  }

    closePosting(postingID: number, postingUpdateStatusDto: any): Observable<PostingDto> {
        let route = `/postings/${postingID}/close`;
        return this.apiService.putToApi(route, postingUpdateStatusDto);
    }

    deletePosting(postingID: number): any {
        let route = `/postings/${postingID}/delete`;
        return this.apiService.deleteToApi(route);
    }

    getPostingsDetailedByYear(year: number): Observable<PostingDto[]> {
        let route = `/postings-activity/${year}`;
        return this.apiService.getFromApi(route);
    }

    getTradesFromPostingID(postingID: number): Observable<TradeDto[]> {
        let route = `/postings/${postingID}/trades`;
        return this.apiService.getFromApi(route);
    }
}
