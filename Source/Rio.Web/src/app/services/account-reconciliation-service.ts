import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterYearDto } from "../shared/models/water-year-dto";
import { AccountReconciliationDto } from '../shared/models/account-reconciliation-dto';
import { Injectable } from '@angular/core';

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
