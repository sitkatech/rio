import { Injectable } from '@angular/core';
import { OAuthStorage } from 'angular-oauth2-oidc';
import { CookieService } from 'ngx-cookie-service';


@Injectable({
  providedIn: 'root'
})
export class CookieStorageService extends OAuthStorage {
  constructor(private cookieService: CookieService) {
    super();
  }

  getItem(key: string): string | null {
      return this.cookieService.get(key);
  }

  removeItem(key: string): void {
    return this.cookieService.delete(key);
  }

  removeAll(): void {
    return this.cookieService.deleteAll();
  }

  setItem(key: string, data: string): void {
    // Explicitly setting the path because sometimes the cookie service was setting it automatically based on the current route, resulting in multiple cookies of the same name.
    // If you need set a cookie in another way, you may need another method
    return this.cookieService.set(key, data, { path: '/' });
  }
}
