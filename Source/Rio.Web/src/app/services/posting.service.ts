import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { PostingDto } from '../shared/models/posting/posting-dto';
import { PostingUpdateDto } from '../shared/models/posting/posting-update-dto';

@Injectable({
    providedIn: 'root'
})
export class PostingService {

    constructor(private apiService: ApiService) { }

    getPostings(): Observable<PostingDto[]> {
        let route = `/postings`;
        return this.apiService.getFromApi(route);
    }

    getPostingsByUserID(userID: number): Observable<PostingDto[]> {
        let route = `/users/${userID}/postings`;
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

    updatePosting(postingID: number, postingUpdateDto: PostingUpdateDto): Observable<PostingDto>  {
        let route = `/postings/${postingID}/update`;
        return this.apiService.putToApi(route, postingUpdateDto);
    }
}
