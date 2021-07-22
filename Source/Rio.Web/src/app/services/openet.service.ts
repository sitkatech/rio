import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { OpenETSyncHistoryDto } from '../shared/models/openet-sync-history-dto';

@Injectable({
    providedIn: 'root'
})
export class OpenETService {
    constructor(private apiService: ApiService) { }

    public getOpenETSyncHistory(): Observable<Array<OpenETSyncHistoryDto>> {
        const route = "openet-sync-history";
        return this.apiService.getFromApi(route);
      }

    public triggerGoogleBucketRefreshForWaterYearMonth(selectedWaterYear: number): Observable<any> {
        const route = "openet-sync-history/trigger-openet-google-bucket-refresh";
        return this.apiService.postToApi(route, selectedWaterYear)
    }

    public isApiKeyValid(): Observable<boolean> {
        const route = "openet/is-api-key-valid";
        return this.apiService.getFromApi(route);
    }
}