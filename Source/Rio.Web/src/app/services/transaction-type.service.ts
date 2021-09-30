import { Injectable } from '@angular/core';
import { ApiService } from '../shared/services';
import { Observable } from 'rxjs';
import { TransactionTypeDto } from '../shared/models/transaction-type-dto';

@Injectable({
  providedIn: 'root'
})
export class TransactionTypeService {

  constructor(private apiService: ApiService) { }

  public getTransactionTypes(): Observable<TransactionTypeDto[]>{
    return this.apiService.getFromApi('/transaction-types/');
  }

  public getAllocationTypes(): Observable<TransactionTypeDto[]>{
    return this.apiService.getFromApi('/allocation-types/');
  }
}
