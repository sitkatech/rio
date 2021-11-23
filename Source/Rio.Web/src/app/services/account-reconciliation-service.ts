import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { AccountReconciliationDto } from '../shared/generated/model/account-reconciliation-dto';

@Injectable({
  providedIn: 'root'
})
export class AccountReconciliationService {
  constructor(private apiService: ApiService) { }

  getAccountsToBeReconciled(): Observable<Array<AccountReconciliationDto>> {
    let route = `/account-reconciliations`;
    return this.apiService.getFromApi(route);
  }
}
