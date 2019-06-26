import { Component, OnInit, HostListener, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { CookieStorageService } from '../../services/cookies/cookie-storage.service';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
    selector: 'header-nav',
    templateUrl: './header-nav.component.html',
    styleUrls: ['./header-nav.component.scss']
})

export class HeaderNavComponent implements OnInit {
    windowWidth: number;

    @HostListener('window:resize', ['$event'])
    resize(ev?: Event) {
        this.windowWidth = window.innerWidth;
    }

    constructor(
        private authenticationService: AuthenticationService,
        private cookieStorageService: CookieStorageService,
        private cdr: ChangeDetectorRef) {
    }

    ngOnInit() {
    }

    public isAuthenticated(): boolean {
        return this.authenticationService.isAuthenticated();
    }

    public viewManage(): boolean {
        return this.authenticationService.canAdmin();
    }

    public getUserName() {
        return this.authenticationService.currentUser ? this.authenticationService.currentUser.FullName
            : null;
    }

    public login(): void {
        this.authenticationService.login();
    }

    public logout(): void {
        this.authenticationService.logout();

        setTimeout(() => {
            this.cookieStorageService.removeAll();
            this.cdr.detectChanges();
        });
    }
}
