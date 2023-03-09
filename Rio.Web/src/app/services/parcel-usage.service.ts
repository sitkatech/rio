import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { ParcelUsageStagingPreviewDto } from '../shared/generated/model/parcel-usage-staging-preview-dto';

@Injectable({
  providedIn: 'root'
})
export class ParcelUsageService {
  constructor(private apiService: ApiService, private httpClient: HttpClient) { }

  previewStagedParcelUsage(): Observable<ParcelUsageStagingPreviewDto> {
    let route = 'parcel-usages';
    return this.apiService.getFromApi(route);
  }

  publishStagedParcelUsage(): Observable<number> {
    let route = 'parcel-usages';
    return this.apiService.postToApi(route, {});
  }

  deleteStagedParcelUsage(): Observable<number> {
    let route = 'parcel-usages';
    return this.apiService.deleteToApi(route);
  }

  getCsvHeaders(uploadedFile: any): Observable<any[]> {
    let formData = new FormData();
    formData.append("UploadedFile", uploadedFile);

    const programApiRoute = environment.mainAppApiUrl;
    const route = `${programApiRoute}/parcel-usages/csv/headers`;
    var result = this.httpClient.post<any>(route, formData);

    return result;
  }

  uploadParcelUsageCsvToStaging(uploadedFile: any, effectiveDate: string, apnColumnName: string, quantityColumnName: string): Observable<string[]> {
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
