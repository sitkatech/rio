import { Component, Inject } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { environment } from '../environments/environment';
import { Router, RouteConfigLoadStart, RouteConfigLoadEnd, NavigationEnd } from '@angular/router';
import { BusyService } from './shared/services';
import { AuthenticationService } from './services/authentication.service';
import { Title } from '@angular/platform-browser';
import { DOCUMENT } from '@angular/common';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';

declare var require: any

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {

    userClaimsUpsertStarted = false;
    customRichTextTypeID = CustomRichTextTypeEnum.WebsiteFooter;

    constructor(
        private router: Router,
        private oauthService: OAuthService,
        private busyService: BusyService,
        private authenticationService: AuthenticationService,
        private titleService: Title,
        @Inject(DOCUMENT) private _document: HTMLDocument
    ) {
        this.configureOAuthService();
        this.authenticationService.initialLoginSequence();
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

        this.titleService.setTitle(`${environment.platformLongName}`)
        this.setAppFavicon();
    }

    private configureOAuthService() {
        this.oauthService.configure(environment.keystoneAuthConfiguration);
        this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    }

    setAppFavicon(){
        this._document.getElementById('appFavicon').setAttribute('href', "assets/main/favicons/" + environment.faviconFilename);
     }
}
