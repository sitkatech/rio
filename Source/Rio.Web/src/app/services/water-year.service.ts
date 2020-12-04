import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterYearDto } from '../shared/models/openet-sync-history-dto';
import { WaterYearQuickOpenETHistoryDto } from '../shared/models/water-year-quick-open-et-history-dto';

@Injectable({
  providedIn: 'root'
})
export class WaterYearService {
  constructor(private apiService: ApiService) { }

  getWaterYears(): Observable<Array<WaterYearDto>> {
    let route = `/water-years`;
    return this.apiService.getFromApi(route);
  }

  getDefaultWaterYearToDisplay(): Observable<WaterYearDto> {
    let route = `/water-years/default`;
    return this.apiService.getFromApi(route);
  }

  public getAbbreviatedSyncHistoryForWaterYears(): Observable<Array<WaterYearQuickOpenETHistoryDto>> {
    let route = `/water-years/abbreviated-open-et-sync-history`;
    return this.apiService.getFromApi(route);
  }

  public finalizeWaterYear(waterYearID: number): Observable<WaterYearDto> {
    const route = "/water-year/finalize";
    return this.apiService.putToApi(route, waterYearID);
  }
}
