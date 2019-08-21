import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterTransferDto } from '../shared/models/water-transfer-dto';
import { WaterTransferConfirmDto } from '../shared/models/water-transfer-confirm-dto';

@Injectable({
    providedIn: 'root'
})
export class WaterTransferService {

  constructor(private apiService: ApiService) { }

  public getWaterTransferFromWaterTransferID(waterTransferID: number): Observable<WaterTransferDto> {
      let route = `/water-transfers/${waterTransferID}`;
      return this.apiService.getFromApi(route);
  }

  public confirmTransfer(waterTransferID: number, waterTransferConfirmDto: WaterTransferConfirmDto): Observable<WaterTransferDto>  {
    let route = `/water-transfers/${waterTransferID}/confirm`;
    return this.apiService.postToApi(route, waterTransferConfirmDto);
  }
}
