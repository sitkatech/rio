import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { OAuthStorage } from 'angular-oauth2-oidc';
import { CookieService } from 'ngx-cookie';


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
    return this.cookieService.remove(key);
  }

  removeAll(): void {
    return this.cookieService.removeAll();
  }

  setItem(key: string, data: string): void {
    return this.cookieService.put(key, data)
  }
}
