import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { CookieStorageService } from '../services/cookies/cookie-storage.service';


@Injectable({
    providedIn: 'root'
})
export class AuthInterceptor implements HttpInterceptor {

    constructor(private injector: Injector) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let nextEvent: HttpRequest<any> = req;

        if (req.url.indexOf('keystone') < 0 && req.url.indexOf('WFS') < 0) {
            const cookieStorageService = this.injector.get(CookieStorageService);
            const token = cookieStorageService.getItem('access_token');
            if (token) {
                nextEvent = req.clone({
                headers: req.headers.set('Authorization', `Bearer ${token}`)
                });
            } else {
                console.warn('no auth token');
            }
        }

        return next.handle(nextEvent);
    }
}
