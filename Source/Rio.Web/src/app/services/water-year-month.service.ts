import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterYearDto } from "../shared/models/water-year-dto";
import { WaterYearQuickOpenETHistoryDto } from '../shared/models/water-year-quick-open-et-history-dto';
import { OpenETSyncHistoryDto } from '../shared/models/openet-sync-history-dto';
import { WaterYearMonthDto } from '../shared/models/water-year-month-dto';


@Injectable({
  providedIn: 'root'
})
export class WaterYearMonthService {
  constructor(private apiService: ApiService) { }

  getWaterYearMonths(): Observable<Array<WaterYearMonthDto>> {
    let route = `/water-year-months`;
    return this.apiService.getFromApi(route);
  }

  getWaterYearMonthsForCurrentDateOrEarlier(): Observable<Array<WaterYearMonthDto>> {
    let route = `/water-year-months/current-date-or-earlier`
    return this.apiService.getFromApi(route);
  }

  // getNonFinalizedWaterYears(): Observable<Array<WaterYearDto>> {
  //   let route = `/water-years/non-finalized`;
  //   return this.apiService.getFromApi(route);
  // }

  public getMostRecentSyncHistoryForWaterYearMonthsThatHaveBeenUpdated(): Observable<Array<OpenETSyncHistoryDto>> {
    let route = `/water-year-months/most-recent-sync-history`;
    return this.apiService.getFromApi(route);
  }

  public finalizeWaterYearMonth(waterYearMonthID: number): Observable<WaterYearMonthDto> {
    const route = "/water-year-month/finalize";
    return this.apiService.putToApi(route, waterYearMonthID);
  }
}
