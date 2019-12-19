import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { AccountDto } from 'src/app/shared/models/account/account-dto';
import { AccountUpdateDto } from 'src/app/shared/models/account/account-update-dto';

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
}
