import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterTransferDto } from '../shared/models/water-transfer-dto';
import { WaterTransferRegisterDto } from '../shared/models/water-transfer-register-dto';

@Injectable({
    providedIn: 'root'
})
export class WaterTransferService {

  constructor(private apiService: ApiService) { }

  public getWaterTransferFromWaterTransferID(waterTransferID: number): Observable<WaterTransferDto> {
      let route = `/water-transfers/${waterTransferID}`;
      return this.apiService.getFromApi(route);
  }

  public registerTransfer(waterTransferID: number, waterTransferRegisterDto: WaterTransferRegisterDto): Observable<WaterTransferDto>  {
    let route = `/water-transfers/${waterTransferID}/register`;
    return this.apiService.postToApi(route, waterTransferRegisterDto);
  }
}
