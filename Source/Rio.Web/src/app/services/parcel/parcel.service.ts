import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { BoundingBoxDto } from 'src/app/shared/models/bounding-box-dto';
import { ParcelAllocationUpsertWrapperDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-wrapper-dto.';
import { ParcelAllocationAndUsageDto } from 'src/app/shared/models/parcel/parcel-allocation-and-usage-dto';
import { ParcelAllocationDto } from 'src/app/shared/models/parcel/parcel-allocation-dto';
import { ParcelMonthlyEvapotranspirationDto } from 'src/app/shared/models/parcel/parcel-monthly-evapotranspiration-dto';
import { ParcelAllocationUpsertDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-dto.';
import { ParcelOwnershipDto } from 'src/app/shared/models/parcel/parcel-ownership-dto';
import { ParcelChangeOwnerDto } from 'src/app/shared/models/parcel/parcel-change-owner-dto';

@Injectable({
  providedIn: 'root'
})
export class ParcelService {
  constructor(private apiService: ApiService) { }

  getParcelsByAccountID(accountID: number, year: number): Observable<any[]> {
    let route = `/accounts/${accountID}/parcels/${year}`;
    return this.apiService.getFromApi(route);
  }

  getParcelsByUserID(userID: number, year: number): Observable<ParcelDto[]> {
    let route = `/users/${userID}/parcels/${year}`;
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

  updateAnnualAllocations(parcelID: number, model: ParcelAllocationUpsertWrapperDto): Observable<any[]> {
    let route = `/parcels/${parcelID}/updateAnnualAllocations`;
    return this.apiService.postToApi(route, model);
  }

  bulkSetAnnualAllocations(model: ParcelAllocationUpsertDto): Observable<any[]> {
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

  getParcelsWithLandOwners(year: number): Observable<Array<ParcelDto>> {
    let route = `/parcels/getParcelsWithLandOwners/${year}`;
    return this.apiService.getFromApi(route);
  }

  getParcelAllocationAndUsagesByYear(year: number): Observable<Array<ParcelAllocationAndUsageDto>> {
    console.log(year);
    let route = `/parcels/getParcelsWithAllocationAndUsage/${year}`;
    return this.apiService.getFromApi(route);
  }

  getParcelOwnershipHistory(parcelID: number): Observable<Array<ParcelOwnershipDto>> {
    let route = `/parcels/${parcelID}/getOwnershipHistory`
    return this.apiService.getFromApi(route);
  }

  changeParcelOwner(parcelID: number, model: ParcelChangeOwnerDto): Observable<any[]> {
    let route = `/parcels/${parcelID}/changeOwner`
    return this.apiService.postToApi(route, model);
  }
}
