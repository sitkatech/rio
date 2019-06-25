import { Component, OnInit, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserDto } from '../../models';
import { Subject, throwError } from 'rxjs';
import { UserService } from 'src/app/services/user/user.service';
import { catchError, map } from 'rxjs/operators';
import { CookieStorageService } from '../../services/cookies/cookie-storage.service';

@Component({
    selector: 'header-nav',
    templateUrl: './header-nav.component.html',
    styleUrls: ['./header-nav.component.scss']
})

export class HeaderNavComponent implements OnInit {
    windowWidth: number;
    public currentUser: UserDto;

    @HostListener('window:resize', ['$event'])
    resize(ev?: Event) {
        this.windowWidth = window.innerWidth;
    }

    constructor(
        private oauthService: OAuthService,
        private cookieStorageService: CookieStorageService,
        private userService: UserService) {
    }

    ngOnInit() {
        this.getAuthenticationUser();
    }

    public isAuthenticated(): boolean {
        return this.oauthService.hasValidAccessToken();
    }

    public viewManage(): boolean {
        return this.canAdmin();
    }

    public getUserName() {
        const claims = this.oauthService.getIdentityClaims() as any;
        if (!claims || !claims.hasOwnProperty('given_name')) {
          return null;
        }
        return claims.given_name;
    }

    public getAuthenticationUser(): void {
        console.log("getting claims");
        if (!this.isAuthenticated()) {
            console.log("not authenticated");
            return;
        }

        const claims = this.oauthService.getIdentityClaims() as any;
        console.log(claims);

        const currentUserGlobalID = claims.hasOwnProperty('sub') ? claims['sub'] : null

        if (!currentUserGlobalID) {
            return;
        }

        this.userService.getUserFromGlobalID(currentUserGlobalID)
        .subscribe(resource => {
            this.currentUser = resource instanceof Array
                ? null
                : resource as UserDto;
        });
    }

    public handleUnauthorized(): void {
        this.logout();
    }

    public login(): void {
        this.oauthService.initImplicitFlow();
    }

    public logout(): void {
        this.oauthService.logOut();

        setTimeout(() => {
            this.cookieStorageService.removeAll();
        });
    }

    public canAdmin(): boolean {
        const role = this.currentUser && this.currentUser.Role
            ? this.currentUser.Role.RoleID
            : null;
        return role === 3 || role === 4; // SitkaAdmin or Admin; todo: need to add enums
    }

}
