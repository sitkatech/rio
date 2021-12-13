import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from './user/user.service';
import { Subject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { CookieStorageService } from '../shared/services/cookies/cookie-storage.service';
import { Router, NavigationEnd, NavigationStart } from '@angular/router';
import { RoleEnum } from '../shared/models/enums/role.enum';
import { AlertService } from '../shared/services/alert.service';
import { Alert } from '../shared/models/alert';
import { AlertContext } from '../shared/models/enums/alert-context.enum';
import { environment } from 'src/environments/environment';
import { AccountSimpleDto } from '../shared/generated/model/account-simple-dto';
import { UserCreateDto } from '../shared/generated/model/user-create-dto';
import { UserDto } from '../shared/generated/model/user-dto';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private currentUser: UserDto;

  private getUserObservable: any;

  private _currentUserSetSubject = new Subject<UserDto>();
  public currentUserSetObservable = this._currentUserSetSubject.asObservable();


  constructor(private router: Router,
    private oauthService: OAuthService,
    private cookieStorageService: CookieStorageService,
    private userService: UserService,
    private alertService: AlertService) {
    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe((e: NavigationEnd) => {
        if (this.isAuthenticated()) {
          this.getGlobalIDFromClaimsAndAttemptToSetUserObservableAndCreateUserIfNecessary();
        } else {
          this.currentUser = null;
          this._currentUserSetSubject.next(null);
        }
      });

    // check for a currentUser at NavigationStart so that authorization-based guards can work with promises.
    this.router.events
      .pipe(filter(e => e instanceof NavigationStart))
      .subscribe((e: NavigationStart) => {
        this.checkAuthentication();
      })
  }

  public checkAuthentication() {
    if (this.isAuthenticated() && !this.currentUser) {
      console.log("Authenticated but no user found...");
      this.getGlobalIDFromClaimsAndAttemptToSetUserObservableAndCreateUserIfNecessary();
    }
  }

  public getGlobalIDFromClaimsAndAttemptToSetUserObservableAndCreateUserIfNecessary() {
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
  }

  private getUserCallback(user: UserDto) {
    this.currentUser = user;

    if (this.isUserRoleDisabled(this.currentUser)) {
      this._currentUserSetSubject.next(this.currentUser);
      return;
    }

    this.userService.listAccountsByUserID(this.currentUser.UserID).subscribe(result => {
      this._availableAccounts = result;
      this._currentUserSetSubject.next(this.currentUser);
    })
  }

  private _availableAccounts: Array<AccountSimpleDto>;

  public getAvailableAccounts(): Array<AccountSimpleDto> {
    return this._availableAccounts;
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
    this.forcedLogout();
  }

  public forcedLogout() {
    sessionStorage["authRedirectUrl"] = window.location.href;
    this.logout();
  }

  public login() {
    this.oauthService.initCodeFlow();
  }

  public createAccount() {
    localStorage.setItem("loginOnReturn", "true");
    window.location.href = `${environment.keystoneAuthConfiguration.issuer}/Account/Register?${this.getClientIDAndRedirectUrlForKeystone()}`;
  }

  public getClientIDAndRedirectUrlForKeystone() {
    return `ClientID=${environment.keystoneAuthConfiguration.clientId}&RedirectUrl=${encodeURIComponent(environment.createAccountRedirectUrl)}`;
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

  isUserADemoUser(user: UserDto): boolean {
    let role = user && user.Role
      ? user.Role.RoleID
      : null;
    return role === RoleEnum.DemoUser;
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

  public isCurrentUserALandOwner(): boolean {
    return this.isUserALandOwner(this.currentUser);
  }

  public isCurrentUserADemoUser(): boolean {
    return this.isUserADemoUser(this.currentUser);
  }

  public isCurrentUserALandOwnerOrDemoUser(): boolean {
    return this.isUserALandOwnerOrDemoUser(this.currentUser);
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
