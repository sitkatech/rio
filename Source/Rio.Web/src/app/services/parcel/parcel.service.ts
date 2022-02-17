import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { BoundingBoxDto } from 'src/app/shared/generated/model/bounding-box-dto';
import { ParcelWaterSupplyAndUsageDto } from 'src/app/shared/generated/model/parcel-water-supply-and-usage-dto';
import { ParcelChangeOwnerDto } from 'src/app/shared/generated/model/parcel-change-owner-dto';
import { ParcelDto } from 'src/app/shared/generated/model/parcel-dto';
import { ParcelLedgerDto } from 'src/app/shared/generated/model/parcel-ledger-dto';
import { ParcelOwnershipDto } from 'src/app/shared/generated/model/parcel-ownership-dto';
import { ParcelUpdateExpectedResultsDto } from 'src/app/shared/generated/model/parcel-update-expected-results-dto';
import { ParcelLayerUpdateDto } from 'src/app/shared/generated/model/parcel-layer-update-dto';
import { ParcelSimpleDto } from 'src/app/shared/generated/model/parcel-simple-dto';

@Injectable({
  providedIn: 'root'
})
export class ParcelService {
  constructor(private apiService: ApiService, private httpClient: HttpClient) { }

  getParcelsByAccountID(accountID: number, year: number): Observable<any[]> {
    let route = `/accounts/${accountID}/parcels/${year}`;
    return this.apiService.getFromApi(route);
  }

  getParcelsByUserID(userID: number, year: number): Observable<ParcelSimpleDto[]> {
    let route = `/users/${userID}/parcels/${year}`;
    return this.apiService.getFromApi(route);
  }

  getParcelByParcelID(parcelID: number): Observable<ParcelDto> {
    let route = `/parcels/${parcelID}`;
    return this.apiService.getFromApi(route);
  }

  getParcelsByTagID(tagID: number): Observable<Array<ParcelSimpleDto>> {
    let route = `/parcels/${tagID}/listByTagID`;
    return this.apiService.getFromApi(route);
  }

  searchParcelNumber(parcelNumber: string): Observable<string> {
    let route = `/parcels/search/${parcelNumber}`;
    return this.apiService.getFromApi(route);
  }

  getParcelLedgerEntriesByParcelID(parcelID: number): Observable<Array<ParcelLedgerDto>> {
    let route = `parcels/${parcelID}/getLedgerEntries`;
    return this.apiService.getFromApi(route);
  }

  getBoundingBoxByParcelIDs(parcelIDs: Array<number>): Observable<BoundingBoxDto> {
    let route = `/parcels/getBoundingBox`;
    let parcelIDListDto = { parcelIDs: parcelIDs };
    return this.apiService.postToApi(route, parcelIDListDto);
  }

  getParcelsWithLandOwners(year: number): Observable<Array<ParcelDto>> {
    let route = `/parcels/getParcelsWithLandOwners/${year}`;
    return this.apiService.getFromApi(route);
  }

  // TODO: needs to get Parcel with Tags
  getParcelWaterSupplyAndUsagesByYear(year: number): Observable<Array<ParcelWaterSupplyAndUsageDto>> {
    let route = `/parcels/getParcelsWithWaterSupplyAndUsage/${year}`;
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

  public uploadGDB(gdbInputFile: any): Observable<any> {
    // we need to do it this way because the apiService.postToApi does a json.stringify, which won't work for input type="file"
    let formData = new FormData();
    formData.append("InputFile", gdbInputFile);
    const programApiRoute = environment.mainAppApiUrl;
    const route = `${programApiRoute}/parcels/uploadGDB`;
    var result = this.httpClient.post<any>(
      route,
      formData
    );

    return result;
  }

  getGDBPreview(model: ParcelLayerUpdateDto): Observable<ParcelUpdateExpectedResultsDto> {
    let route = `/parcels/previewGDBChanges`;
    return this.apiService.postToApi(route, model);
  }

  enactGDBChanges(waterYearID: number) : Observable<any> {
    let route = `/parcels/enactGDBChanges`;
    return this.apiService.postToApi(route, waterYearID);
  }

  getParcelGDBCommonMappingToParcelStagingColumn(): Observable<any> {
    let route = `/parcels/parcelGDBCommonMappingToParcelStagingColumn`;
    return this.apiService.getFromApi(route);
  }

  getInactiveParcels(): Observable<Array<ParcelDto>> {
    let route = `/parcels/inactive`;
    return this.apiService.getFromApi(route);
  }
}
