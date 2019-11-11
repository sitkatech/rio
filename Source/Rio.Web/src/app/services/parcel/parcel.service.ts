import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { BoundingBoxDto } from 'src/app/shared/models/bounding-box-dto';
import { ParcelAllocationUpsertWrapperDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-wrapper-dto.';
import { ParcelAllocationAndUsageDto } from 'src/app/shared/models/parcel/parcel-allocation-and-usage-dto';
import { ParcelAllocationDto } from 'src/app/shared/models/parcel/parcel-allocation-dto';
import { ParcelMonthlyEvapotranspirationDto } from 'src/app/shared/models/parcel/parcel-monthly-evapotranspiration-dto.1';
import { ParcelAllocationUpsertDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-dto.';

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

  getWaterYears(): Observable<Array<number>> {
    let route = `/getWaterYears`;
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
  
  bulkSetAnnualAllocations(model: ParcelAllocationUpsertDto): Observable<any[]>  {
    let route = `/parcels/bulkSetAnnualParcelAllocation`;
    return this.apiService.postToApi(route, model);
  }
  
  getParcelAllocations(parcelID: number): Observable<Array<ParcelAllocationDto>> {
    let route = `/parcels/${parcelID}/getAllocations`;
    return this.apiService.getFromApi(route);
  }
  
  getWaterUsage(parcelID: number): Observable<Array<ParcelMonthlyEvapotranspirationDto>> {
    let route = `/parcels/${parcelID}/getWaterUsage`;
    return this.apiService.getFromApi(route);
  }

  getParcelsWithLandOwners(): Observable<Array<ParcelDto>> {
    let route = `/parcels/getParcelsWithLandOwners`;
    return this.apiService.getFromApi(route);
  }

  getParcelAllocationAndUsagesByYear(year: number): Observable<Array<ParcelAllocationAndUsageDto>> {
    let route = `/parcels/getParcelsWithAllocationAndUsage/${year}`;
    return this.apiService.getFromApi(route);
  }
}
