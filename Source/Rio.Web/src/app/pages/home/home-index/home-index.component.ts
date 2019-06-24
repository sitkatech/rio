import {Component, OnInit} from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
    selector: 'app-home-index',
    templateUrl: './home-index.component.html',
    styleUrls: ['./home-index.component.scss']
})
export class HomeIndexComponent implements OnInit {

    constructor(private authenticationService: OAuthService) {
    }

    public ngOnInit(): void {
    }

    public authenticated(): boolean {
        return this.authenticationService.hasValidAccessToken();
    }

    public userCanEdit(): boolean {
        return this.authenticationService.hasValidAccessToken();
    }
}
