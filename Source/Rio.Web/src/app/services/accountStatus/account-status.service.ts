import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from 'src/app/shared/services';

@Injectable({
  providedIn: 'root'
})
export class AccountStatusService {
 
  constructor(private apiService: ApiService) { }
  
  getAccountStatuses(): Observable<any[]> {
    let route = `/accountStatus`;
    return this.apiService.getFromApi(route);
}
}
