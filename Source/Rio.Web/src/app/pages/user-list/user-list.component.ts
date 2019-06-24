import { Component, OnInit, ViewChildren, QueryList, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { UserDto } from 'src/app/shared/models';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
    selector: 'rio-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {

    users: UserDto[];

    displayInviteUserDialog: boolean = false;

    userToEdit = {};
    displayEditUserDialog: boolean = false;

    constructor(private oauthService: OAuthService, private cdr: ChangeDetectorRef, private userService: UserService) { }

    ngOnInit() {
        setTimeout(() => {
            this.getData();
        });
    }

    getData() {
        var currentUserGlobalID = ((this.oauthService.getIdentityClaims() || {}) as any).sub;
        this.userService.getUsers().subscribe(result => {
            this.users = result;
            this.cdr.detectChanges();
        });

        this.userService.getUserFromGlobalID(currentUserGlobalID).subscribe(result => {

        });
    }

    sortLabels(fo1, fo2): number {
        if (fo1.label < fo2.label) {
            return -1;
        } else if (fo1.label > fo2.label) {
            return 1;
        } else {
            return 0;
        }
    }
}
