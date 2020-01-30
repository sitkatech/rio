import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CookieStorageService } from '../../services/cookies/cookie-storage.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UnauthenticatedAccessGuard implements CanActivate {

  constructor(private cookieStorageService: CookieStorageService, private router: Router, private authenticationService: AuthenticationService) {
  }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    const token = this.cookieStorageService.getItem('access_token');
    if (token) {
      return true;
    } else {
      sessionStorage["authRedirectUrl"] = state.url;
      this.authenticationService.login()
      return false;
    }
  }
}

export class AllowTradeGuard implements CanActivate {

  constructor() {
  }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return environment.allowTrading
  }
}