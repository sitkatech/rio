import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from './user/user.service';
import { UserDto } from '../shared/models';
import { Subject, throwError, BehaviorSubject, Observable } from 'rxjs';
import { catchError, map, filter } from 'rxjs/operators';
import { CookieStorageService } from '../shared/services/cookies/cookie-storage.service';
import { isNullOrUndefined } from 'util';
import { Router, NavigationEnd, NavigationStart } from '@angular/router';
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
              this.alertService.clearAlerts();
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

    // check for a currentUser at NavigationStart so that authorization-based guards can work with promises.
    this.router.events
      .pipe(filter(e => e instanceof NavigationStart))
      .subscribe((e: NavigationStart) => {
        if (this.isAuthenticated() && !this.currentUser) {
          var claims = this.oauthService.getIdentityClaims();
          var globalID = claims["sub"];
          console.log("Authenticated but no user found...")
          this.getUserObservable = this.userService.getUserFromGlobalID(globalID).subscribe(user => {
            this.currentUser = user;
            this._currentUserSetSubject.next(this.currentUser);
          });
        }
      })


    this._currentAccountSubject = new BehaviorSubject<AccountSimpleDto>(undefined);
    this._availableAccountsSubject = new BehaviorSubject<AccountSimpleDto[]>(undefined);
  }

  private updateCurrentAccountSubject() {
    const activeAccountAsJson = window.localStorage.getItem('activeAccount');

    if (!isNullOrUndefined(activeAccountAsJson) && activeAccountAsJson !== "undefined") {
      // if the saved account is valid for this user, make it the current active account. Otherwise clear it from local storage.
      let initialActiveAccount = JSON.parse(activeAccountAsJson);
      if (initialActiveAccount && this.getAvailableAccounts().map(x => x.AccountID).includes(initialActiveAccount.AccountID)) {
        this._currentAccountSubject.next(initialActiveAccount);
      } else {
        window.localStorage.removeItem("activeAccount");
      }
    }

    let currentAccounts = this.getAvailableAccounts();
    if (!this._currentAccountSubject.value || !currentAccounts.some(x => x.AccountID == this._currentAccountSubject.value.AccountID)) {
      // no current account or no longer have access to current account: leave active account undefined if manager or if you don't have any accounts, default to first on list otherwise.
      if (this.getAvailableAccounts() && this.currentUser.Role.RoleID != RoleEnum.Admin) {
        this._currentAccountSubject.next(this.getAvailableAccounts()[0]);
      } else {
        this._currentAccountSubject.next(undefined);
      }
    }
  }

  private getUserCallback(user: UserDto) {
    this.currentUser = user;
    this._currentUserSetSubject.next(this.currentUser);
    if (!this.isUserRoleDisabled(this.currentUser)) {
      this.userService.listAccountsByUserID(this.currentUser.UserID).subscribe(result => {
        this._availableAccounts = result;
        this._availableAccountsSubject.next(result);
        this.updateCurrentAccountSubject();
      })
    }
  }

  private _availableAccounts: Array<AccountSimpleDto>;
  //There are many places where we don't use the subject version defined here.
  //It may be better to use the observable in some of those places, but that's beyond the current scope of work
  private _availableAccountsSubject: BehaviorSubject<AccountSimpleDto[]>;

  public getAvailableAccounts(): Array<AccountSimpleDto> {
    return this._availableAccounts;
  }

  public getAvailableAccountsObservable(): Observable<AccountSimpleDto[]> {
    return this._availableAccountsSubject.asObservable();
  }

  // Returns the observable (read-only) part of this subject
  public getActiveAccount(): Observable<AccountSimpleDto> {
    return this._currentAccountSubject.asObservable();
  }

  // Stores the new company value in local storage and pushes it to the subject
  public setActiveAccount(account: AccountSimpleDto, navigate: boolean = true) {
    window.localStorage.setItem('activeAccount', JSON.stringify(account));
    this._currentAccountSubject.next(account);
    if (this.router.url !== "/landowner-dashboard" && navigate) {
      this.router.navigateByUrl("/landowner-dashboard");
    }
  }

  public refreshUserInfo(user: UserDto) {
    this.getUserCallback(user);
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
    localStorage.setItem("loginOnReturn", "true");
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

  public isUserALandOwnerOrDemoUser(user: UserDto): boolean {
    let role = user && user.Role
      ? user.Role.RoleID
      : null;
    return role === RoleEnum.LandOwner || role === RoleEnum.DemoUser;
  }

  public isUserAnAdministrator(user: UserDto): boolean {
    const role = user && user.Role
      ? user.Role.RoleID
      : null;
    return role === RoleEnum.Admin;
  }

  public isUserADemoUserOrAdministrator(user: UserDto): boolean {
    const role = user && user.Role
      ? user.Role.RoleID
      : null;
    return role === RoleEnum.DemoUser || role === RoleEnum.Admin;
  }

  public isUserALandOwnerOrDemoUserOrAdministrator(user: UserDto): boolean {
    let role = user && user.Role
      ? user.Role.RoleID
      : null;
    return role === RoleEnum.DemoUser || role === RoleEnum.Admin || role == RoleEnum.LandOwner;
  }

  public isCurrentUserNullOrUndefined(): boolean {
    return !this.currentUser;
  }

  public isCurrentUserDisabled(): boolean {
    return this.isUserRoleDisabled(this.currentUser);
  }

  public isCurrentUserUnassigned(): boolean {
    return this.isUserUnassigned(this.currentUser);
  }

  public isCurrentUserAnAdministrator(): boolean {
    return this.isUserAnAdministrator(this.currentUser);
  }

  public isCurrentUserADemoUserOrAdministrator(): boolean {
    return this.isUserADemoUserOrAdministrator(this.currentUser);
  }

  public isCurrentUserALandOwnerOrDemoUserOrAdministrator(): boolean {
    return this.isUserALandOwnerOrDemoUserOrAdministrator(this.currentUser);
  }

  public isUserUnassigned(user: UserDto): boolean {
    const role = user && user.Role
      ? user.Role.RoleID
      : null;
    return role === RoleEnum.Unassigned;
  }

  public isUserRoleDisabled(user: UserDto): boolean {
    const role = user && user.Role
      ? user.Role.RoleID
      : null;
    return role === RoleEnum.Disabled;
  }

  public hasCurrentUserAcknowledgedDisclaimer(): boolean {
    return this.currentUser != null && this.currentUser.DisclaimerAcknowledgedDate != null;
  }
}
