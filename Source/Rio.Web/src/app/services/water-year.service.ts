import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterYearDto } from "../shared/models/water-year-dto";

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

  public getWaterYearForCurrentYearAndVariableYearsBack(numYearsBackToInclude: number): Observable<Array<WaterYearDto>> {
    let route = `water-years/current-and-variable-previous/${numYearsBackToInclude}`
    return this.apiService.getFromApi(route);
  }
}

