import { Component, OnInit, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';

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

    constructor(private oauthService: OAuthService, private router: Router) {
    }

    ngOnInit() {
    }

    public viewExplore(): boolean {
        return this.oauthService.hasValidAccessToken();
    }

    public viewManage(): boolean {
        return this.oauthService.hasValidAccessToken();
    }
}
