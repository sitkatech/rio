import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { PostingDto } from '../shared/models/posting/posting-dto';

@Injectable({
    providedIn: 'root'
})
export class PostingService {

    constructor(private apiService: ApiService) { }

    getPostings(): Observable<PostingDto[]> {
        let route = `/postings`;
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

    deletePosting(postingID: number): void {
        let route = `/postings/${postingID}/delete`;
        this.apiService.getFromApi(route);
    }

    updatePosting(postingID: number, postingUpdateDto: any): Observable<PostingDto>  {
        let route = `/postings/${postingID}/update`;
        return this.apiService.putToApi(route, postingUpdateDto);
    }
}
