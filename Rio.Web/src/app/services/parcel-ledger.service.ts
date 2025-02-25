import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { ParcelLedgerCreateDto } from '../shared/generated/model/parcel-ledger-create-dto';
import { TransactionHistoryDto } from '../shared/generated/model/transaction-history-dto';

@Injectable({
  providedIn: 'root'
})
export class ParcelLedgerService {
  constructor(private apiService: ApiService, private httpClient: HttpClient) { }

  getAllTransactionHistory(): Observable<Array<TransactionHistoryDto>> {
    let route = `parcel-ledgers/transaction-history`;
    return this.apiService.getFromApi(route);
  }

  newTransaction(model: ParcelLedgerCreateDto): Observable<any[]> {
    let route = `parcel-ledgers`;
    return this.apiService.postToApi(route, model);
  }

  newBulkTransaction(model: ParcelLedgerCreateDto): Observable<any[]> {
    let route = `parcel-ledgers/bulk`;
    return this.apiService.postToApi(route, model);
  }

  newCSVUploadTransaction(uploadedFile: any, effectiveDate: string, waterTypeID: string): Observable<any[]> {
    let formData = new FormData();
    formData.append("UploadedFile", uploadedFile);
    formData.append("EffectiveDate", effectiveDate);
    formData.append("WaterTypeID", waterTypeID);

    const programApiRoute = environment.mainAppApiUrl;
    const route = `${programApiRoute}/parcel-ledgers/csv`;
    var result = this.httpClient.post<any>(
      route,
      formData
    );

    return result;
  }
}
