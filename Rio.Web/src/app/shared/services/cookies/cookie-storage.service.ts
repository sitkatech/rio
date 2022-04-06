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
    return this.cookieService.delete(key, '/');
  }

  removeAll(): void {
    return this.cookieService.deleteAll('/');
  }

  setItem(key: string, data: string, expires: number | Date = 1): void {
    this.cookieService.set(key, data, expires, "/");
  }
}
