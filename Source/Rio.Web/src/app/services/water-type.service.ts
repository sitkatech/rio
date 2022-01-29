import { Injectable } from '@angular/core';
import { ApiService } from '../shared/services';
import { Observable } from 'rxjs';
import { WaterTypeDto } from '../shared/generated/model/water-type-dto';

@Injectable({
  providedIn: 'root'
})
export class WaterTypeService {

  constructor(private apiService: ApiService) { }

  public getWaterTypes(): Observable<WaterTypeDto[]>{
    return this.apiService.getFromApi('/water-types/');
  }

  public mergeWaterTypes(waterTypes: WaterTypeDto[]): Observable<any>{
    return this.apiService.putToApi('/water-types/', waterTypes);
  }
}
