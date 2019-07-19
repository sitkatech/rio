import { Injectable } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterYearTransactionDto } from 'src/app/shared/models/water-year-transaction-dto';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    constructor(private apiService: ApiService) { }

    inviteUser(userInviteDto: any): Observable<UserDto>  {
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

    getWaterYearAcresTransactedForUser(userID: number): Observable<Array<WaterYearTransactionDto>> {
        let route = `/yearly-transaction-summary/${userID}`;
        return this.apiService.getFromApi(route);
    }

    updateUser(userID: number, userUpdateDto: any): Observable<UserDto>  {
        let route = `/users/${userID}`;
        return this.apiService.putToApi(route, userUpdateDto);
    }
}
