import { Injectable } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterTransferDto } from 'src/app/shared/models/water-transfer-dto';
import { WaterUsageDto } from 'src/app/shared/models/water-usage-dto';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    getWaterUsageByUserID(userID: number): Observable<WaterUsageDto[]> {
        let route = `/users/${userID}/water-usage`;
        return this.apiService.getFromApi(route);
    }
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
}
