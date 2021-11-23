import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { WaterAllocationOverviewDto } from 'src/app/shared/models/water-usage-dto';
import { HttpClient } from '@angular/common/http';
import { AccountDto } from 'src/app/shared/generated/model/account-dto';
import { AccountEditUsersDto } from 'src/app/shared/generated/model/account-edit-users-dto';
import { AccountUpdateDto } from 'src/app/shared/generated/model/account-update-dto';
import { ParcelLedgerDto } from 'src/app/shared/generated/model/parcel-ledger-dto';
import { ParcelSimpleDto } from 'src/app/shared/generated/model/parcel-simple-dto';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  
  constructor(private apiService: ApiService,
    private httpClient: HttpClient) { }

  public listAllAccounts(): Observable<Array<AccountDto>>{
    const route = "/accounts"
    return this.apiService.getFromApi(route);
  }

  public getAccountByID(accountID): Observable<AccountDto>{
    const route = `/account/${accountID}`
    return this.apiService.getFromApi(route);
  }

  public getAccountByAccountNumber(accountNumber): Observable<AccountDto> {
    const route= `/account/account-number/${accountNumber}`
    return this.apiService.getFromApi(route);
  }

  public getAccountByAccountVerificationKey(accountVerificationKey: string): Observable<any>{
    const route = `/account/account-verification-key/${accountVerificationKey}`
    return this.apiService.getFromApi(route);
  }

  public updateAccount(accountID: number, accountUpdateDto: any): Observable<AccountDto> {
      let route = `/account/${accountID}`;
      return this.apiService.putToApi(route, accountUpdateDto);
  }

  createAccount(model: AccountUpdateDto): Observable<AccountDto> {
    let route = '/account/new';
    return this.apiService.postToApi(route, model);
  }

  editUsers(accountID: number, model: AccountEditUsersDto): Observable<AccountDto> {
    let route = `/account/${accountID}/edit-users`;
    return this.apiService.putToApi(route, model);
  }

  getParcelLedgersByAccountID(accountID: number): Observable<Array<ParcelLedgerDto>> {
    let route = `/accounts/${accountID}/parcel-ledgers`;
    return this.apiService.getFromApi(route);
  }

  getWaterUsageOverview(year: number): Observable<WaterAllocationOverviewDto> {
    let route = `/accounts/water-usage-overview/${year}`;
      return this.apiService.getFromApi(route);
  }


  getParcelsInAccountReconciliationByAccountID(accountID:number): Observable<Array<ParcelSimpleDto>> {
    let route = `/accounts/${accountID}/account-reconciliation-parcels`;
    return this.apiService.getFromApi(route);
  }
}
