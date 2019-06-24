import { Component, OnInit, HostListener, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { CookieStorageService } from '../../services/cookies/cookie-storage.service';

@Component({
  selector: 'user-authentication',
  templateUrl: './user-authentication.component.html',
  styleUrls: ['./user-authentication.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class UserAuthenticationComponent implements OnInit {

  readonly SIZE_MD = 767;
  windowWidth: number;

  constructor(private cdr: ChangeDetectorRef, private oauthService: OAuthService, private cookieStorageService: CookieStorageService) {
  }

  ngOnInit() {
    this.resize();
  }

  @HostListener('window:resize', ['$event'])
  resize(ev?: Event) {
      this.windowWidth = window.innerWidth;
  }

  hasToken() {
    return this.oauthService.hasValidAccessToken();
  }

  login() {
    this.oauthService.initImplicitFlow();
  }

  logout() {
    this.oauthService.logOut();

    setTimeout(() => {
      this.cookieStorageService.removeAll();
      this.cdr.markForCheck();
    });
  }

  getCurrentGivenName() {
    const claims = this.oauthService.getIdentityClaims() as any;
    if (!claims || !claims.hasOwnProperty('given_name')) {
      return null;
    }
    return claims.given_name;
  }
}
