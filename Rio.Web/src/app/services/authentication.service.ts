import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from './user/user.service';
import { Observable, race, Subject } from 'rxjs';
import { filter, finalize, first } from 'rxjs/operators';
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
import { UserDetailedDto } from '../shared/generated/model/user-detailed-dto';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private currentUser: UserDto;

  private _currentUserSetSubject = new Subject<UserDto>();
  private currentUserSetObservable = this._currentUserSetSubject.asObservable();
  private attemptingToCreateUser: boolean;


  constructor(private router: Router,
    private oauthService: OAuthService,
    private cookieStorageService: CookieStorageService,
    private userService: UserService,
    private alertService: AlertService) {
    this.oauthService.events.subscribe(_ => {
      this.checkAuthentication();
    });

    this.oauthService.events
      .pipe(filter(e => ['token_received'].includes(e.type)))
      .subscribe(e => this.oauthService.loadUserProfile());

    this.oauthService.events
      .pipe(filter(e => ['invalid_nonce_in_state'].includes(e.type)))
      .subscribe(e => {
        //During user creation our nonce can get into an invalid state, but just signing in again mitigates the issue
        if (this.router.url.includes("signin-oidc")) {
          this.oauthService.initCodeFlow();
        }
      });

    this.oauthService.events
      .pipe(filter(e => ['token_refresh_error'].includes(e.type)))
      .subscribe(e => {
        //If we're still authenticated, don't worry about the error
        if (this.isAuthenticated()) {
          return;
        }

        //If we haven't cleared our cookies and done the logout, do so
        var token = this.oauthService.getAccessToken();
        if (token != null && token != undefined && token != "") {
          this.logout();
          return;
        }

        this.router
          .navigateByUrl("/")
          .then(() => this.alertService.pushAlert(new Alert("Your session has been terminated. Please login again.")));
      });

    this.oauthService.setupAutomaticSilentRefresh();
  }

  public initialLoginSequence(): Promise<void> {
    return this.oauthService.loadDiscoveryDocument()
      .then(() => this.oauthService.tryLogin())
      .then(() => {
        if (this.oauthService.hasValidAccessToken()) {
          return Promise.resolve();
        }
        return this.oauthService.silentRefresh().then(() => Promise.resolve());
      }).catch(() => { });
  }

  public checkAuthentication() {
    if (this.isAuthenticated() && !this.currentUser) {
      console.log("Authenticated but no user found...");
      var claims = this.oauthService.getIdentityClaims();
      this.getUser(claims);
    }
  }

  public getUser(claims: any) {
    var globalID = claims["sub"];

    this.userService.getUserFromGlobalID(globalID).subscribe(
      result => { this.updateUser(result); },
      error => { this.onGetUserError(error, claims) }
    );
  }

  private onGetUserError(error: any, claims: any) {
    if (error.status !== 404) {
      this.alertService.pushAlert(new Alert("There was an error logging into the application.", AlertContext.Danger));
      this.router.navigate(['/']);
    }

    if (!this.attemptingToCreateUser) {
      this.attemptingToCreateUser = true;
      this.alertService.clearAlerts();
      const newUser = new UserCreateDto({
        FirstName: claims["given_name"],
        LastName: claims["family_name"],
        Email: claims["email"],
        RoleID: RoleEnum.Unassigned,
        LoginName: claims["login_name"],
        UserGuid: claims["sub"],
      });

      this.userService.createNewUser(newUser).pipe(
        finalize(() => this.attemptingToCreateUser = false)
      ).subscribe(user => {
        this.updateUser(user);
      });
    }
  }

  private updateUser(user: UserDto) {
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
    this.updateUser(user);
  }

  public getCurrentUser(): Observable<UserDto> {
    return race(
      new Observable(subscriber => {
        if (this.currentUser) {
          subscriber.next(this.currentUser);
          subscriber.complete();
        }
      }),
      this.currentUserSetObservable.pipe(first())
    );
  }

  public isAuthenticated(): boolean {
    return this.oauthService.hasValidAccessToken();
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

  public getAuthRedirectUrl() {
    return sessionStorage["authRedirectUrl"];
  }

  public setAuthRedirectUrl(url: string) {
    sessionStorage["authRedirectUrl"] = url;
  }

  public clearAuthRedirectUrl() {
    this.setAuthRedirectUrl("");
  }
}
