import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ParcelService {

    constructor(private apiService: ApiService) { }
        
    getParcelsByUserID(userID: number): Observable<any[]> {
        let route = `/users/${userID}/parcels`;
        return this.apiService.getFromApi(route);
    }
}
