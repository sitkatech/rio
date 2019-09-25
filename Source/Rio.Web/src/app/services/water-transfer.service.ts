import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterTransferDto } from '../shared/models/water-transfer-dto';
import { WaterTransferRegisterDto } from '../shared/models/water-transfer-register-dto';
import { WaterTransferParcelDto } from '../shared/models/water-transfer-parcel-dto';
import { WaterTransferParcelsWrapperDto } from '../shared/models/water-transfer-parcels-wrapper-dto.';

@Injectable({
    providedIn: 'root'
})
export class WaterTransferService {

  constructor(private apiService: ApiService) { }

  public getWaterTransferFromWaterTransferID(waterTransferID: number): Observable<WaterTransferDto> {
      let route = `/water-transfers/${waterTransferID}`;
      return this.apiService.getFromApi(route);
  }

  public getParcelsForWaterTransferID(waterTransferID: number): Observable<Array<WaterTransferParcelDto>> {
      let route = `/water-transfers/${waterTransferID}/parcels`;
      return this.apiService.getFromApi(route);
  }

  public selectParcelsForWaterTransferID(waterTransferID: number, waterTransferParcelsWrapperDto: WaterTransferParcelsWrapperDto): Observable<WaterTransferParcelDto> {
    let route = `/water-transfers/${waterTransferID}/selectParcels`;
    return this.apiService.postToApi(route, waterTransferParcelsWrapperDto);
  }

  public registerTransfer(waterTransferID: number, waterTransferRegisterDto: WaterTransferRegisterDto): Observable<WaterTransferDto>  {
    let route = `/water-transfers/${waterTransferID}/register`;
    return this.apiService.postToApi(route, waterTransferRegisterDto);
  }
}
