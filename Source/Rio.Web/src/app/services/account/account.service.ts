import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { AccountDto } from 'src/app/shared/models/account/account-dto';
import { AccountEditUsersDto } from "src/app/shared/models/account/account-edit-users-dto";
import { AccountUpdateDto } from 'src/app/shared/models/account/account-update-dto';
import { AccountEditUsersComponent } from 'src/app/pages/account-edit-users/account-edit-users.component';
import { WaterUsageDto, WaterAllocationOverviewDto } from 'src/app/shared/models/water-usage-dto';
import { ParcelMonthlyEvapotranspirationDto } from 'src/app/shared/models/parcel/parcel-monthly-evapotranspiration-dto';
import { ParcelMonthlyEvapotranspirationOverrideDto } from 'src/app/shared/models/parcel/parcel-monthly-evapotranspiration-override-dto';
import { ParcelAllocationDto } from 'src/app/shared/models/parcel/parcel-allocation-dto';
import { WaterTransferDto } from 'src/app/shared/models/water-transfer-dto';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  
  constructor(private apiService: ApiService) { }

  public listAllAccounts(): Observable<Array<AccountDto>>{
    const route = "/accounts"
    return this.apiService.getFromApi(route);
  }

  public getAccountByID(accountID): Observable<AccountDto>{
    const route = `/account/${accountID}`
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

  getWaterTransfersByAccountID(accountID: number): Observable<Array<WaterTransferDto>> {
      let route = `/accounts/${accountID}/water-transfers`;
      return this.apiService.getFromApi(route);
  }

  getWaterUsageByAccountID(accountID: number, year: number): Observable<WaterUsageDto[]> {
      let route = `/accounts/${accountID}/water-usage/${year}`;
      return this.apiService.getFromApi(route);
  }
  
  getParcelWaterUsageByAccountID(accountID: number, year: number): Observable<ParcelMonthlyEvapotranspirationDto[]> {
      let route = `/accounts/${accountID}/parcel-water-usage/${year}`;
      return this.apiService.getFromApi(route);
  }
  
  getWaterUsageOverviewByAccountID(accountID: number, year: number): Observable<WaterAllocationOverviewDto> {
      let route = `/accounts/${accountID}/water-usage-overview/${year}`;
      return this.apiService.getFromApi(route);
  }

  getParcelsAllocationsByAccountID(accountID: number, year: number): Observable<Array<ParcelAllocationDto>> {
      let route = `/accounts/${accountID}/getParcelsAllocations/${year}`;
      return this.apiService.getFromApi(route);
  }

  saveParcelMonthlyEvapotranspirationOverrideValues(accountID: number, year: number, model: Array<ParcelMonthlyEvapotranspirationDto>) : Observable<any> {
      let route = `/accounts/${accountID}/${year}/saveParcelMonthlyEvapotranspirationOverrideValues`;
      return this.apiService.putToApi(route, model);
  }
}
