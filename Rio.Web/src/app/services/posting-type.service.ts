import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class PostingTypeService {

    constructor(private apiService: ApiService) { }
        
    getPostingTypes(): Observable<any[]> {
        let route = `/postingTypes`;
        return this.apiService.getFromApi(route);
    }
}
