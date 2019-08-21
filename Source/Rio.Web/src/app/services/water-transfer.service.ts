import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterTransferDto } from '../shared/models/water-year-transaction-dto';

@Injectable({
    providedIn: 'root'
})
export class WaterTransferService {

  constructor(private apiService: ApiService) { }

  public getWaterTransferFromWaterTransferID(waterTransferID: number): Observable<WaterTransferDto> {
      let route = `/water-transfers/${waterTransferID}`;
      return this.apiService.getFromApi(route);
  }
}
