import { Component, OnInit, ViewChildren, QueryList, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { UserDto } from 'src/app/shared/models';

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

    constructor(private cdr: ChangeDetectorRef, private userService: UserService) { }

    ngOnInit() {
        setTimeout(() => {
            this.getData();
        });
    }

    getData() {
        this.userService.getUsers().subscribe(result => {
            this.users = result;
            this.cdr.detectChanges();
        });
    }
}
