import { Injectable } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterTransferDto } from 'src/app/shared/models/water-transfer-dto';
import { WaterUsageDto, WaterAllocationOverviewDto } from 'src/app/shared/models/water-usage-dto';
import { MultiSeriesEntry, SeriesEntry } from "src/app/shared/models/series-entry";
import { ParcelAllocationDto } from 'src/app/shared/models/parcel/parcel-allocation-dto';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    constructor(private apiService: ApiService) { }

    inviteUser(userInviteDto: any): Observable<UserDto> {
        let route = `/users/invite`;
        return this.apiService.postToApi(route, userInviteDto);
    }

    getUsers(): Observable<UserDto[]> {
        let route = `/users`;
        return this.apiService.getFromApi(route);
    }

    getUserFromUserID(userID: number): Observable<UserDto> {
        let route = `/users/${userID}`;
        return this.apiService.getFromApi(route);
    }

    getUserFromGlobalID(globalID: string): Observable<UserDto> {
        let route = `/user-claims/${globalID}`;
        return this.apiService.getFromApi(route);
    }

    getWaterTransfersByUserID(userID: number): Observable<Array<WaterTransferDto>> {
        let route = `/users/${userID}/water-transfers`;
        return this.apiService.getFromApi(route);
    }

    updateUser(userID: number, userUpdateDto: any): Observable<UserDto> {
        let route = `/users/${userID}`;
        return this.apiService.putToApi(route, userUpdateDto);
    }

    getWaterUsageByUserID(userID: number, year: number): Observable<WaterUsageDto[]> {
        let route = `/users/${userID}/water-usage/${year}`;
        return this.apiService.getFromApi(route);
    }
    
    getWaterUsageOverviewByUserID(userID: number): Observable<WaterAllocationOverviewDto> {
        let route = `/users/${userID}/water-usage-overview`;
        return this.apiService.getFromApi(route);
    }

    getLandownerUsageReportByYear(year: number): Observable<UserDto[]> {
        let route = `/landowner-usage-report/${year}`;
        return this.apiService.getFromApi(route);
    }

    getParcelsAllocationsByUserID(userID: number, year: number): Observable<Array<ParcelAllocationDto>> {
        let route = `/users/${userID}/getParcelsAllocations/${year}`;
        return this.apiService.getFromApi(route);
      }
    
    
}
