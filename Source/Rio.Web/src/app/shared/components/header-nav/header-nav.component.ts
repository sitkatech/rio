import { Component, OnInit, HostListener, ChangeDetectorRef, ChangeDetectionStrategy, OnDestroy } from '@angular/core';
import { CookieStorageService } from '../../services/cookies/cookie-storage.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from '../../models';

@Component({
    selector: 'header-nav',
    templateUrl: './header-nav.component.html',
    styleUrls: ['./header-nav.component.scss']
})

export class HeaderNavComponent implements OnInit, OnDestroy {
    private watchUserChangeSubscription: any;
    private currentUser: UserDto;

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
        this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
            this.currentUser = currentUser;
        });
    }

    ngOnDestroy() {
        this.watchUserChangeSubscription.unsubscribe();
        this.authenticationService.dispose();
        this.cdr.detach();
    }

    public isAuthenticated(): boolean {
        return this.authenticationService.isAuthenticated();
    }

    public isLandOwner(): boolean {
        return this.authenticationService.isUserALandOwner(this.currentUser);
    }

    public isAdministrator(): boolean {
        return this.authenticationService.isUserAnAdministrator(this.currentUser);
    }

    public getUserName() {
        return this.currentUser ? this.currentUser.FullName
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
