import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { AccountIncludeParcelsDto } from 'src/app/shared/generated/model/account-include-parcels-dto';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { LandownerUsageReportDto } from 'src/app/shared/generated/model/landowner-usage-report-dto';
import { UnassignedUserReportDto } from 'src/app/shared/generated/model/unassigned-user-report-dto';
import { UserCreateDto } from 'src/app/shared/generated/model/user-create-dto';
import { UserDetailedDto } from 'src/app/shared/generated/model/user-detailed-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { UserPartnerInviteDto } from 'src/app/shared/generated/model/user-partner-invite-dto';
import { UserUpsertDto } from 'src/app/shared/generated/model/user-upsert-dto';
import { UserEditAccountsDto } from 'src/app/shared/generated/model/user-edit-accounts-dto';

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

    updateUser(userID: number, userUpsertDto: UserUpsertDto): Observable<UserDto> {
        let route = `/users/${userID}`;
        return this.apiService.putToApi(route, userUpsertDto);
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
