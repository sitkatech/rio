import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterTransferDto } from '../shared/models/water-transfer-dto';
import { WaterTransferRegistrationDto } from '../shared/models/water-transfer-registration-dto';
import { WaterTransferRegistrationParcelDto } from '../shared/models/water-transfer-registration-parcel-dto';
import { WaterTransferRegistrationSimpleDto } from '../shared/models/water-transfer-registration-simple-dto';

@Injectable({
    providedIn: 'root'
})
export class WaterTransferService {

  constructor(private apiService: ApiService) { }

  public getWaterTransferFromWaterTransferID(waterTransferID: number): Observable<WaterTransferDto> {
      let route = `/water-transfers/${waterTransferID}`;
      return this.apiService.getFromApi(route);
  }

  public getWaterTransferRegistrationsFromWaterTransferID(waterTransferID: number): Observable<Array<WaterTransferRegistrationSimpleDto>> {
      let route = `/water-transfers/${waterTransferID}/registrations`;
      return this.apiService.getFromApi(route);
  }

  public getParcelsForWaterTransferIDAndUserID(waterTransferID: number, userID: number): Observable<Array<WaterTransferRegistrationParcelDto>> {
      let route = `/water-transfers/${waterTransferID}/parcels/${userID}`;
      return this.apiService.getFromApi(route);
  }

  public selectParcelsForWaterTransferID(waterTransferID: number, waterTransferParcelsWrapperDto: WaterTransferRegistrationDto): Observable<WaterTransferRegistrationParcelDto> {
    let route = `/water-transfers/${waterTransferID}/selectParcels`;
    return this.apiService.postToApi(route, waterTransferParcelsWrapperDto);
  }

  public registerTransfer(waterTransferID: number, waterTransferRegistrationDto: WaterTransferRegistrationDto): Observable<WaterTransferDto>  {
    let route = `/water-transfers/${waterTransferID}/register`;
    return this.apiService.postToApi(route, waterTransferRegistrationDto);
  }
}
