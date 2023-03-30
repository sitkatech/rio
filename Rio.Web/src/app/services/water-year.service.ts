import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterYearDto } from '../shared/generated/model/water-year-dto';
import { OverconsumptionRateUpsertDto } from '../shared/generated/model/overconsumption-rate-upsert-dto';

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

  updateOverconsumptionRate(waterYearID: number, overconsumptionRateUpsertDto: OverconsumptionRateUpsertDto): Observable<any> {
    let route = `/water-years/${waterYearID}/overconsumption-rate`;
    return this.apiService.putToApi(route, overconsumptionRateUpsertDto);
  }

  public getWaterYearForCurrentYearAndVariableYearsBack(numYearsBackToInclude: number): Observable<Array<WaterYearDto>> {
    let route = `water-years/current-and-variable-previous/${numYearsBackToInclude}`
    return this.apiService.getFromApi(route);
  }
}

