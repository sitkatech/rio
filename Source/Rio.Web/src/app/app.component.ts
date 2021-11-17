import { Component, Inject } from '@angular/core';
import { OAuthService, OAuthSuccessEvent } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { Subject, Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { CookieStorageService } from './shared/services/cookies/cookie-storage.service';
import { Router, RouteConfigLoadStart, RouteConfigLoadEnd, NavigationEnd } from '@angular/router';
import { BusyService } from './shared/services';
import { AuthenticationService } from './services/authentication.service';
import { Title } from '@angular/platform-browser';
import { DOCUMENT } from '@angular/common';

declare var require: any

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {

    userClaimsUpsertStarted = false;
    ignoreSessionTerminated = false;

    constructor(private router: Router, 
        private oauthService: OAuthService, 
        private busyService: BusyService, 
        private authenticationService: AuthenticationService, 
        private titleService: Title, 
        @Inject(DOCUMENT) private _document: HTMLDocument) {
    }

    ngOnInit() {
        this.router.events.subscribe((event: any) => {
            if (event instanceof RouteConfigLoadStart) { // lazy loaded route started
                this.busyService.setBusy(true);
            } else if (event instanceof RouteConfigLoadEnd) { // lazy loaded route ended
                this.busyService.setBusy(false);
            } else if (event instanceof NavigationEnd) {
                window.scrollTo(0, 0);
            }
        });
        
        this.configureAuthService().subscribe(() => {
            this.oauthService.tryLogin();
        });

        this.titleService.setTitle(`${environment.leadOrganizationShortName} ${environment.platformShortName}`)
        this.setAppFavicon();
    }

    private configureAuthService(): Observable<void> {
        const subject = new Subject<void>();
        this.oauthService.configure(environment.keystoneAuthConfiguration);
        this.oauthService.setupAutomaticSilentRefresh();
        this.oauthService.tokenValidationHandler = new JwksValidationHandler();
        this.oauthService.loadDiscoveryDocument();
        this.oauthService.events
            .subscribe((e) => {
                console.log(e.type);
                switch (e.type) {
                    case 'discovery_document_loaded':
                        console.log("discovery_document_loaded");
                        if ((e as OAuthSuccessEvent).info) {
                            subject.next();
                            subject.complete();
                        }
                        break;
                    case 'token_received':
                        console.log("token_received");
                        subject.next();
                        subject.complete();
                        this.authenticationService.checkAuthentication();
                        break;
                    case 'token_refreshed':
                        subject.next();
                        subject.complete();
                        //this.authenticationService.checkAuthentication();
                        break;
                    case 'token_refresh_error':
                        console.log("token_refresh_error");
                        this.authenticationService.logout();
                        break;
                    // case 'session_changed':
                    //     console.log("session_changed");
                    //     // when the user logins from no-tenant URL and then jumps to the tenant URL,
                    //     // the oAuthService triggers a session-changed followed by a session_terminated...
                    //     // however the token still works as expected.
                    //     // ATTENTION: Need to verify that on session expiration (and hence session_terminated) this session_changed doesn't get called...
                    //     this.ignoreSessionTerminated = true;
                    //     break;
                    // case 'session_terminated':
                    //     if (!this.ignoreSessionTerminated) {
                    //         console.warn('Your session has been terminated!');
                    //         this.cookieStorageService.removeAll();
                    //     }
                    //     debugger;
                    //     this.ignoreSessionTerminated = false;
                    //     break;
                }

            });

        return subject.asObservable();
    }

    setAppFavicon(){
        this._document.getElementById('appFavicon').setAttribute('href', "assets/main/favicons/" + environment.faviconFilename);
     }
}
