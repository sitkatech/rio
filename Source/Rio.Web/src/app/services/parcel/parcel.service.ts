import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { BoundingBoxDto } from 'src/app/shared/models/bounding-box-dto';
import { ParcelAllocationAndUsageDto } from 'src/app/shared/models/parcel/parcel-allocation-and-usage-dto';
import { ParcelAllocationDto } from 'src/app/shared/models/parcel/parcel-allocation-dto';
import { ParcelMonthlyEvapotranspirationDto } from 'src/app/shared/models/parcel/parcel-monthly-evapotranspiration-dto';
import { ParcelAllocationUpsertDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-dto.';
import { ParcelOwnershipDto } from 'src/app/shared/models/parcel/parcel-ownership-dto';
import { ParcelChangeOwnerDto } from 'src/app/shared/models/parcel/parcel-change-owner-dto';
import { ParcelAllocationHistoryDto } from 'src/app/shared/models/parcel/parcel-allocation-history-dto';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { ParcelLayerUpdateDto } from 'src/app/pages/parcel-update-layer/parcel-update-layer.component';
import { ParcelUpdateExpectedResultsDto } from 'src/app/shared/models/parcel-update-expected-results-dto';
import { ParcelStatusEnum } from 'src/app/shared/models/enums/parcel-status-enum';
import { ParcelWithStatusDto } from 'src/app/shared/models/parcel/parcel-with-status-dto';

@Injectable({
  providedIn: 'root'
})
export class ParcelService {
  constructor(private apiService: ApiService, private httpClient: HttpClient) { }

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

  getBoundingBoxByParcelIDs(parcelIDs: Array<number>): Observable<BoundingBoxDto> {
    let route = `/parcels/getBoundingBox`;
    let parcelIDListDto = { parcelIDs: parcelIDs };
    return this.apiService.postToApi(route, parcelIDListDto);
  }

  mergeParcelAllocations(parcelID: number, model: ParcelAllocationDto[]): Observable<any> {
    let route = `parcels/${parcelID}/mergeParcelAllocations`;
    return this.apiService.postToApi(route, model);
  }

  bulkSetAnnualAllocations(model: ParcelAllocationUpsertDto, userID: number): Observable<any[]> {
    let route = `/parcels/${userID}/bulkSetAnnualParcelAllocation`;
    return this.apiService.postToApi(route, model);
  }

  bulkSetAnnualAllocationsFileUpload(file: any, waterYear: number, allocationTypeID: number): Observable<any> {
    const apiHostName = environment.apiHostName
    const route = `https://${apiHostName}/parcels/${waterYear}/${allocationTypeID}/bulkSetAnnualParcelAllocationFileUpload`;
    var result = this.httpClient.post<any>(
      route,
      file, // Send the File Blob as the POST body.
      {
        // NOTE: Because we are posting a Blob (File is a specialized Blob
        // object) as the POST body, we have to include the Content-Type
        // header. If we don't, the server will try to parse the body as
        // plain text.
        headers: {
          "Content-Type": "application/vnd.ms-excel"
        },
        params: {
          clientFilename: file.name,
          mimeType: "application/vnd.ms-excel"
        }
      }
    );

    return result;
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
    let route = `/parcels/getParcelsWithAllocationAndUsage/${year}`;
    return this.apiService.getFromApi(route);
  }

  getParcelOwnershipHistory(parcelID: number): Observable<Array<ParcelOwnershipDto>> {
    let route = `/parcels/${parcelID}/getOwnershipHistory`
    return this.apiService.getFromApi(route);
  }

  getParcelAllocationHistory(): Observable<Array<ParcelAllocationHistoryDto>> {
    let route = `/parcels/getParcelAllocationHistory`
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
    const apiHostName = environment.apiHostName;
    const route = `https://${apiHostName}/parcels/uploadGDB`;
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
