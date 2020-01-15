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
import { AlertService } from '../shared/services/alert.service';
import { Alert } from '../shared/models/alert';
import { AlertContext } from '../shared/models/enums/alert-context.enum';
import { UserCreateDto } from '../shared/models/user/user-create-dto';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private currentUser: UserDto;

  private getUserObservable: any;

  private _currentUserSetSubject = new Subject<UserDto>();
  public currentUserSetObservable = this._currentUserSetSubject.asObservable();

  private _currentAccountSubject: BehaviorSubject<AccountSimpleDto>;


  constructor(private router: Router,
    private oauthService: OAuthService,
    private cookieStorageService: CookieStorageService,
    private userService: UserService,
    private alertService: AlertService) {
    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe((e: NavigationEnd) => {
        if (this.isAuthenticated()) {
          var claims = this.oauthService.getIdentityClaims();
          var globalID = claims["sub"];

          this.getUserObservable = this.userService.getUserFromGlobalID(globalID).subscribe(result => {
            this.getUserCallback(result);
          }, error => {
            if (error.status !== 404) {
              this.alertService.pushAlert(new Alert("There was an error logging into the application.", AlertContext.Danger));
              this.router.navigate(['/']);
            } else {
              const newUser = new UserCreateDto({
                FirstName: claims["given_name"],
                LastName: claims["family_name"],
                Email: claims["email"],
                RoleID: RoleEnum.Unassigned,
                LoginName: claims["login_name"],
                UserGuid: claims["sub"],
              });

              this.userService.createNewUser(newUser).subscribe(user => {
                this.getUserCallback(user);
              })

            }
          });

        } else {
          this.currentUser = null;
          this._currentUserSetSubject.next(null);
        }
      });

    // todo: I think this should just be new up the behavior subject as undefined and then do the localStorage stuff after the account list comes back.

    const activeAccountAsJson = window.localStorage.getItem('activeAccount');


    if (!isNullOrUndefined(activeAccountAsJson) && activeAccountAsJson !== "undefined") {
      // todo: check that activeAccountAsJson is in the list of available accounts. 
      let initialActiveAccount = JSON.parse(activeAccountAsJson);
      if (initialActiveAccount) {
        this._currentAccountSubject = new BehaviorSubject<AccountSimpleDto>(initialActiveAccount);
      }
    }
    else {
      if (this.getAvailableAccounts()) {
        this._currentAccountSubject = new BehaviorSubject<AccountSimpleDto>(this.getAvailableAccounts()[0]);
      } else {
        this._currentAccountSubject = new BehaviorSubject<AccountSimpleDto>(undefined);
      }
    }
  }

  private getUserCallback(user: UserDto) {
    this.currentUser = user;
    this._currentUserSetSubject.next(this.currentUser);

    this.userService.listAccountsByUserID(this.currentUser.UserID).subscribe(result => {
      this._availableAccounts = result;
      if (!this._currentAccountSubject.value) {
        this.setActiveAccount(this._availableAccounts[0])
      }
    })
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
    this.oauthService.initImplicitFlow();
  }


  public createAccount() {
    const redirectUrl = encodeURIComponent(environment.createAccountRedirectUrl);
    window.location.href = `${environment.createAccountUrl}${redirectUrl}`;
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

  public isUserUnassigned(user: UserDto): boolean {
    const role = user && user.Role
      ? user.Role.RoleID
      : null;
    return role === RoleEnum.Unassigned;
  }
}
