import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { ParcelUsageCsvResponseDto } from '../shared/models/parcel-usage-csv-response-dto';

@Injectable({
  providedIn: 'root'
})
export class ParcelUsageService {
  constructor(private apiService: ApiService, private httpClient: HttpClient) { }

  getCsvHeaders(uploadedFile: any): Observable<any[]> {
    let formData = new FormData();
    formData.append("UploadedFile", uploadedFile);

    const programApiRoute = environment.mainAppApiUrl;
    const route = `${programApiRoute}/parcel-usages/csv/headers`;
    var result = this.httpClient.post<any>(route, formData);

    return result;
  }

  uploadParcelUsageCsvToStaging(uploadedFile: any, effectiveDate: string, apnColumnName: string, quantityColumnName: string): Observable<ParcelUsageCsvResponseDto> {
    let formData = new FormData();
    formData.append("UploadedFile", uploadedFile);
    formData.append("EffectiveDate", effectiveDate);
    formData.append("APNColumnName", apnColumnName);
    formData.append("QuantityColumnName", quantityColumnName);

    const programApiRoute = environment.mainAppApiUrl;
    const route = `${programApiRoute}/parcel-usages/csv`;
    var result = this.httpClient.post<any>(route, formData);

    return result;
  }
}
