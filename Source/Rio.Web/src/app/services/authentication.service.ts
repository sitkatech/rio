import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from './user/user.service';
import { UserDto } from '../shared/models';
import { Subject, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { CookieStorageService } from '../shared/services/cookies/cookie-storage.service';
import { isNullOrUndefined } from 'util';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  public currentUser: UserDto;
  private personLoadedSubject = new Subject<UserDto>();

  constructor(
    private oauthService: OAuthService,
    private cookieStorageService: CookieStorageService,
    private userService: UserService,
  ) {
  }

  public isAuthenticated(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  public getAuthenticationUser(): void {
    console.log("getting claims");
    if (!this.isAuthenticated()) {
      console.log("not authenticated");
      return;
    }

    const claims = this.oauthService.getIdentityClaims() as any;
    const currentUserGlobalID = claims.hasOwnProperty('sub') ? claims['sub'] : null

    if (!currentUserGlobalID) {
      return;
    }

    this.userService.getUserFromGlobalID(currentUserGlobalID)
      .pipe(
        map(response => {
          const user: UserDto = response instanceof Array
            ? null
            : response as UserDto;
          this.personLoadedSubject.next(user);
          return user;
        }),
        catchError(err => {
          this.handleUnauthorized();
          return throwError(err);
        }),
      )
      .subscribe(person => {
        this.currentUser = person;
      });
  }

  public handleUnauthorized(): void {
    this.logout();
  }

  public login() {
    this.oauthService.initImplicitFlow();
  }

  public logout() {
    this.oauthService.logOut();

    setTimeout(() => {
      this.cookieStorageService.removeAll();
    });
  }

  public isLandOwner(): boolean {
    return this.isUserALandOwner(this.currentUser);
  }

  public isUserALandOwner(user: UserDto): boolean {
    let role = user && user.Role
      ? user.Role.RoleID
      : null;
    return role === 2; // Landowner; todo: need to add enums
  }

  public isAdministrator(): boolean {
    const role = this.currentUser && this.currentUser.Role
      ? this.currentUser.Role.RoleID
      : null;
    return role === 1; // Admin; todo: need to add enums
  }
}
