import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterTransferDto } from '../shared/generated/model/water-transfer-dto';
import { WaterTransferRegistrationDto } from '../shared/generated/model/water-transfer-registration-dto';
import { WaterTransferRegistrationParcelUpsertDto } from '../shared/generated/model/water-transfer-registration-parcel-upsert-dto';
import { WaterTransferRegistrationSimpleDto } from '../shared/generated/model/water-transfer-registration-simple-dto';
import { WaterTransferDetailedDto } from '../shared/generated/model/water-transfer-detailed-dto';

@Injectable({
    providedIn: 'root'
})
export class WaterTransferService {
  constructor(private apiService: ApiService) { }

  public getWaterTransferFromWaterTransferID(waterTransferID: number): Observable<WaterTransferDetailedDto> {
      let route = `/water-transfers/${waterTransferID}`;
      return this.apiService.getFromApi(route);
  }

  public getWaterTransferRegistrationsFromWaterTransferID(waterTransferID: number): Observable<Array<WaterTransferRegistrationSimpleDto>> {
      let route = `/water-transfers/${waterTransferID}/registrations`;
      return this.apiService.getFromApi(route);
  }

  public getParcelsForWaterTransferIDAndUserID(waterTransferID: number, userID: number): Observable<Array<WaterTransferRegistrationParcelUpsertDto>> {
      let route = `/water-transfers/${waterTransferID}/parcels/${userID}`;
      return this.apiService.getFromApi(route);
  }

  public selectParcelsForWaterTransferID(waterTransferID: number, waterTransferRegistrationDto: WaterTransferRegistrationDto): Observable<WaterTransferRegistrationParcelUpsertDto> {
    let route = `/water-transfers/${waterTransferID}/selectParcels`;
    return this.apiService.postToApi(route, waterTransferRegistrationDto);
  }

  public registerTransfer(waterTransferID: number, waterTransferRegistrationDto: WaterTransferRegistrationDto): Observable<WaterTransferDto>  {
    let route = `/water-transfers/${waterTransferID}/register`;
    return this.apiService.postToApi(route, waterTransferRegistrationDto);
  }
  
  public cancelTrade(waterTransferID: number, waterTransferRegistrationDto: WaterTransferRegistrationDto): Observable<WaterTransferDto> {
    let route = `/water-transfers/${waterTransferID}/cancel`;
    return this.apiService.postToApi(route, waterTransferRegistrationDto);
  }
}
