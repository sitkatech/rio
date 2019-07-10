import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { BoundingBoxDto } from 'src/app/shared/models/bounding-box-dto';
import { ParcelAllocationAndConsumptionDto } from 'src/app/shared/models/parcel/parcel-allocation-and-consumption-dto';
import { ParcelAllocationUpsertWrapperDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-wrapper-dto.';

@Injectable({
  providedIn: 'root'
})
export class ParcelService {
  constructor(private apiService: ApiService) { }

  getParcelsByUserID(userID: number): Observable<any[]> {
    let route = `/users/${userID}/parcels`;
    return this.apiService.getFromApi(route);
  }

  getParcelByParcelID(parcelID: number): Observable<ParcelDto> {
    let route = `/parcels/${parcelID}`;
    return this.apiService.getFromApi(route);
  }

  getBoundingBoxByParcelIDs(parcelIDs: Array<number>): Observable<BoundingBoxDto> {
    let route = `/parcels/getBoundingBox`;
    let parcelIDListDto = { parcelIDs: parcelIDs };
    return this.apiService.postToApi(route, parcelIDListDto);
  }

  updateAnnualAllocations(parcelID: number, model: ParcelAllocationUpsertWrapperDto): Observable<any[]>  {
    let route = `/parcels/${parcelID}/updateAnnualAllocations`;
    return this.apiService.postToApi(route, model);
  }
  getParcelAllocationAndConsumption(parcelID: number): Observable<Array<ParcelAllocationAndConsumptionDto>> {
    let route = `/parcels/${parcelID}/getAllocationAndConsumption`;
    return this.apiService.getFromApi(route);
  }

  getParcelAllocationAndConsumptionByUserID(userID: number): Observable<any[]> {
    let route = `/users/${userID}/getParcelsAllocationAndConsumption`;
    return this.apiService.getFromApi(route);
  }

  getParcelsWithLandOwners(): Observable<Array<ParcelDto>> {
    let route = `/parcels/getParcelsWithLandOwners`;
    return this.apiService.getFromApi(route);
  }
}
