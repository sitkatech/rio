import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { OpenETSyncWaterYearStatusDto } from '../shared/models/openet-sync-water-year-status-dto';
import { OpenETSyncHistoryDto } from '../shared/models/openet-sync-history-dto';

@Injectable({
    providedIn: 'root'
})
export class OpenETService {
    constructor(private apiService: ApiService) { }

    public listAllOpenETSyncWaterYearStatus(): Observable<Array<OpenETSyncWaterYearStatusDto>> {
        const route = "/openet-sync-water-year-status"
        return this.apiService.getFromApi(route);
    }

    public finalizeOpenETSyncWaterYearStatus(openETSyncWaterStatusID: number): Observable<OpenETSyncWaterYearStatusDto> {
        const route = "/openet-sync-water-year-status/finalize";
        return this.apiService.putToApi(route, openETSyncWaterStatusID);
    }

    public getInProgressOpenETSyncHistory(): Observable<Array<OpenETSyncHistoryDto>> {
        const route = "openet-sync-history/current-in-progress";
        return this.apiService.getFromApi(route);
    }

    public triggerGoogleBucketRefreshForWaterYear(selectedWaterYear: number): Observable<any> {
        const route = "openet-sync-water-year-status/trigger-openet-google-bucket-refresh";
        return this.apiService.postToApi(route, selectedWaterYear)
    }
}