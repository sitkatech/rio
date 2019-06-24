import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CookieStorageService } from '../../services/cookies/cookie-storage.service';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable({
  providedIn: 'root'
})
export class UnauthenticatedAccessGuard implements CanActivate {

  constructor(private cookieStorageService: CookieStorageService, private router: Router, private oauthService: OAuthService) {
  }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
      const token = this.cookieStorageService.getItem('access_token');
      if (token) {
          return true;
      } else {
          this.oauthService.initImplicitFlow()
          return false;
      }
  }
}