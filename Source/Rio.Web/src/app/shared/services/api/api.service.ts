import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiRouteService } from '../api-route/api-route.service';
import { Router } from '@angular/router';
import { Observable, throwError as _throw } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { BusyService } from '../busy/busy.service';
import { AlertService } from '../alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { OAuthService } from 'angular-oauth2-oidc';
import { AlertContext } from '../../models/enums/alert-context.enum';


@Injectable({
    providedIn: 'root'
})
export class ApiService {

    constructor(private busyService: BusyService, private apiRoute: ApiRouteService, private http: HttpClient, private alertService: AlertService, private oauthService: OAuthService, private router: Router, ) {
    }

    postToApi(relativeRoute: string, data: any): Observable<any> {

        this.busyService.setBusy(true);

        if (relativeRoute.startsWith('/')) {
            relativeRoute = relativeRoute.substring(1, relativeRoute.length);
        }

        data = JSON.parse(JSON.stringify(data, this.requestCleaner));

        const baseRoute = this.apiRoute.getRoute();
        const route = `${baseRoute}/${relativeRoute}`;

        return this.http.post(route, data)
            .pipe(
                map((response: any) => {
                    return this.handleResponse(response);
                }),
                catchError((error: any) => {
                    return this.handleError(error);
                })
            );
    }

    getFromApi(relativeRoute: string): Observable<any> {
        let token = this.oauthService.getAccessToken();
        const headers =  new HttpHeaders().set('Authorization', `Bearer ${token}`);
        this.busyService.setBusy(true);

        if (relativeRoute.startsWith('/')) {
            relativeRoute = relativeRoute.substring(1, relativeRoute.length);
        }

        const baseRoute = this.apiRoute.getRoute();
        const route = `${baseRoute}/${relativeRoute}`;
        const result = this.http.get(route, { headers: headers })
            .pipe(
                map((response: any) => {
                    return this.handleResponse(response);
                }),
                catchError((error: any) => {
                    return this.handleError(error);
                })
            );
        return result;
    }

    putToApi(relativeRoute: string, data: any): Observable<any> {

        this.busyService.setBusy(true);

        if (relativeRoute.startsWith('/')) {
            relativeRoute = relativeRoute.substring(1, relativeRoute.length);
        }

        data = JSON.parse(JSON.stringify(data, this.requestCleaner));

        const baseRoute = this.apiRoute.getRoute();
        const route = `${baseRoute}/${relativeRoute}`;

        return this.http.put(route, data)
            .pipe(
                map((response: any) => {
                    return this.handleResponse(response);
                }),
                catchError((error: any) => {
                    return this.handleError(error);
                })
            );
    }

    deleteToApi(relativeRoute: string): Observable<any> {

        this.busyService.setBusy(true);
        if (relativeRoute.startsWith('/')) {
            relativeRoute = relativeRoute.substring(1, relativeRoute.length);
        }
        const baseRoute = this.apiRoute.getRoute();
        const route = `${baseRoute}/${relativeRoute}`;

        return this.http.delete(route)
            .pipe(
                map((response: any) => {
                    return this.handleResponse(response);
                }),
                catchError((error: any) => {
                    return this.handleError(error);
                })
            );
    }

    handleResponse(response: any): Observable<any> {
        this.busyService.setBusy(false);
        return response;
    }

    sendErrorToHandleError(error) {
        return this.handleError(error);
    }

    private handleError(error: any, supressErrorMessage = false, clearBusyGlobally = true): Observable<any> {
        if (clearBusyGlobally) {
            this.busyService.setBusy(false);
        }

        if (!supressErrorMessage) {
            if (error && (error.status === 401)) {
                this.alertService.pushAlert(new Alert("Access token expired..."));
                this.oauthService.initCodeFlow();
            } else if (error && (error.status === 403)) {
                this.alertService.pushNotFoundUnauthorizedAlert();
                this.router.navigate(["/"]);
            } else if (error.error && typeof error.error === 'string') {
                this.alertService.pushNotFoundUnauthorizedAlert();
                this.alertService.pushAlert(new Alert(error.error));
            } else if (error.error && error.status === 404) {
                // let the caller handle not found appropriate to whatever it was doing
            } else if (error.error && !(error.error instanceof ProgressEvent)) {
                for (const key of Object.keys(error.error)) {
                    // FIXME: will break if errror.error[key] is not a string[]
                    const newLocal = new Alert((error.error[key] as string[]).map((fe: string) => { return key + ": " + fe; }).join(","));
                    this.alertService.pushAlert(newLocal);
                }
            } else {
                this.alertService.pushAlert(new Alert("Oops! Something went wrong and we couldn't complete the action..."));
            }
        }

        return _throw(error);
    }

    private requestCleaner(name: string, val: any) {
        if ((name || '').toString().indexOf('$') === 0) {
            return undefined; // remove from data
        } else {
            return val; // return as is
        }
    }
}