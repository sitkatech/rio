import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from './user/user.service';
import { UserDto } from '../shared/models';
import { Subject, throwError, BehaviorSubject, Observable } from 'rxjs';
import { catchError, map, filter } from 'rxjs/operators';
import { CookieStorageService } from '../shared/services/cookies/cookie-storage.service';
import { isNullOrUndefined } from 'util';
import { Router, NavigationEnd } from '@angular/router';
import { RoleEnum } from '../shared/models/enums/role.enum';
import { AccountSimpleDto } from '../shared/models/account/account-simple-dto';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private currentUser: UserDto;

  private getUserObservable: any;

  private _currentUserSetSubject = new Subject<UserDto>();
  public currentUserSetObservable = this._currentUserSetSubject.asObservable();

  private _currentAccountSubject: Subject<AccountSimpleDto>;


  constructor(private router: Router, private oauthService: OAuthService, private cookieStorageService: CookieStorageService, private userService: UserService) {
    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe((e: NavigationEnd) => {
        if (this.isAuthenticated()) {
          var claims = this.oauthService.getIdentityClaims();
          var globalID = claims["sub"];

          this.getUserObservable = this.userService.getUserFromGlobalID(globalID).subscribe(result => {
            this.currentUser = result;
            this._currentUserSetSubject.next(this.currentUser);

            this.userService.listAccountsByUserID(this.currentUser.UserID).subscribe(result=>{
              this._availableAccounts = result;
            })
          });

        } else {
          this.currentUser = null;
          this._currentUserSetSubject.next(null);
        }
      });

    const activeAccountAsJson = window.localStorage.getItem('activeAccount');

    if (!isNullOrUndefined(activeAccountAsJson) && activeAccountAsJson !== "undefined") {
      let initialActiveAccount = JSON.parse(activeAccountAsJson);
      if (initialActiveAccount) {
        this._currentAccountSubject = new BehaviorSubject<AccountSimpleDto>(initialActiveAccount);
      }
    }
    else {
      this._currentAccountSubject = new BehaviorSubject<AccountSimpleDto>(this.getAvailableAccounts()[0]);
    }
  }

  private _availableAccounts: Array<AccountSimpleDto>;

  public getAvailableAccounts(): Array<AccountSimpleDto> {
    return this._availableAccounts;
  }

  // Returns the observable (read-only) part of this subject
  public getActiveAccount(): Observable<AccountSimpleDto> {
    return this._currentAccountSubject.asObservable();
  }

  // Stores the new company value in local storage and pushes it to the subject
  public setActiveAccount(account: AccountSimpleDto) {
    window.localStorage.setItem('activeAccount', JSON.stringify(account));
    this._currentAccountSubject.next(account);
  }

  dispose() {
    this.getUserObservable.unsubscribe();
  }

  public isAuthenticated(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  public handleUnauthorized(): void {
    this.logout();
  }

  public login() {
    // add in our default redirect url if none is set
    var redirectUrl = sessionStorage.getItem("authRedirectUrl");
    if (!redirectUrl) {
      sessionStorage["authRedirectUrl"] = "/landowner-dashboard";
    }
    this.oauthService.initImplicitFlow();
  }

  public logout() {
    this.oauthService.logOut();

    setTimeout(() => {
      this.cookieStorageService.removeAll();
    });
  }

  public isUserALandOwner(user: UserDto): boolean {
    let role = user && user.Role
      ? user.Role.RoleID
      : null;
    return role === RoleEnum.LandOwner;
  }

  public isUserAnAdministrator(user: UserDto): boolean {
    const role = user && user.Role
      ? user.Role.RoleID
      : null;
    return role === RoleEnum.Admin;
  }
}
