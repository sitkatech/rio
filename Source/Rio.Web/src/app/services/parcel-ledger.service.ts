import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ParcelLedgerCreateDto } from '../shared/generated/model/parcel-ledger-create-dto';

@Injectable({
  providedIn: 'root'
})
export class ParcelLedgerService {
  constructor(private apiService: ApiService, private httpClient: HttpClient) { }

  newTransaction(model: ParcelLedgerCreateDto): Observable<any[]> {
    let route = `parcel-ledgers/new`;
    return this.apiService.postToApi(route, model);
  }

  newBulkTransaction(model: ParcelLedgerCreateDto): Observable<any[]> {
    let route = `parcel-ledgers/bulk-new`;
    return this.apiService.postToApi(route, model);
  }
}
