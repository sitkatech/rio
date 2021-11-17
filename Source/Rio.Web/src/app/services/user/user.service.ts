import { Injectable } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { AccountSimpleDto } from 'src/app/shared/models/account/account-simple-dto';
import { UserCreateDto } from 'src/app/shared/models/user/user-create-dto';
import { UnassignedUserReportDto } from 'src/app/shared/models/user/unassigned-user-report-dto';
import { UserDetailedDto } from 'src/app/shared/models/user/user-detailed-dto';
import { UserEditAccountsDto } from 'src/app/shared/models/user/user-edit-accounts-dto';
import { AccountIncludeParcelsDto } from 'src/app/shared/models/account/account-include-parcels-dto';
import { UserPartnerInviteDto } from 'src/app/shared/models/user/user-partner-invite-dto';
import { LandownerUsageReportDto } from 'src/app/shared/models/landowner-usage-report';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    constructor(private apiService: ApiService) { }

    inviteUser(userInviteDto: any): Observable<UserDto> {
        let route = `/users/invite`;
        return this.apiService.postToApi(route, userInviteDto);
    }

    invitePartner(userPartnerInviteDto: UserPartnerInviteDto) {
        let route = `/users/invite-partner`;
        return this.apiService.postToApi(route, userPartnerInviteDto);
    }

    createNewUser(userCreateDto: UserCreateDto): Observable<UserDto> {
        let route = `/users/`;
        return this.apiService.postToApi(route, userCreateDto);
    }

    getUsers(): Observable<UserDetailedDto[]> {
        let route = `/users`;
        return this.apiService.getFromApi(route);
    }

    getLandownersAndDemoUsers(): Observable<UserDto[]> {
        let route = '/users/landowners-and-demo-users';
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

    listAccountsByUserID(userID: number): Observable<Array<AccountSimpleDto>> {
        let route = `/user/${userID}/accounts`
        return this.apiService.getFromApi(route);
    }

    listAccountsIncludeParcelsByUserID(userID: number): Observable<Array<AccountIncludeParcelsDto>> {
        let route = `/user/${userID}/accounts-include-parcels`
        return this.apiService.getFromApi(route);
    }

    updateUser(userID: number, userUpdateDto: any): Observable<UserDto> {
        let route = `/users/${userID}`;
        return this.apiService.putToApi(route, userUpdateDto);
    }

    addAccountsToCurrentUser(userEditAccountsDto: UserEditAccountsDto) {
        let route = `/user/add-accounts-via-verification-key`;
        return this.apiService.putToApi(route, userEditAccountsDto);
    }
    
    editAccounts(userID: number, userEditAccountsDto: UserEditAccountsDto): Observable<UserDto> {
        let route = `/users/${userID}/edit-accounts`;
        return this.apiService.putToApi(route, userEditAccountsDto);
    }

    removeAccountByIDForCurrentUser(accountID: number): Observable<any> {
        let route = `/user/remove-account/${accountID}`;
        return this.apiService.deleteToApi(route);
    }

    getLandownerUsageReportByYear(year: number): Observable<LandownerUsageReportDto[]> {
        let route = `/landowner-usage-report/${year}`;
        return this.apiService.getFromApi(route);
    }

    getUnassignedUserReport(): Observable<UnassignedUserReportDto> {
        let route = `/users/unassigned-report`;
        return this.apiService.getFromApi(route);
    }

    setDisclaimerAcknowledgedDate(userID: number): Observable<UserDto> {
        let route = `/users/set-disclaimer-acknowledged-date`
        return this.apiService.putToApi(route, userID);
    }
}
