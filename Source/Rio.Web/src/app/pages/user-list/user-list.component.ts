import { Component, OnInit, ViewChildren, QueryList, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { UserDto } from 'src/app/shared/models';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
    selector: 'rio-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit, OnDestroy {
    private watchUserChangeSubscription: any;
    private currentUser: UserDto;
    public users: UserDto[];

    constructor(private cdr: ChangeDetectorRef, private authenticationService: AuthenticationService, private userService: UserService) { }

    ngOnInit() {
        this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
            this.currentUser = currentUser;
            this.userService.getUsers().subscribe(result => {
                this.users = result;
                this.cdr.detectChanges();
            });
        });
    }

    ngOnDestroy() {
        this.watchUserChangeSubscription.unsubscribe();
        this.authenticationService.dispose();
        this.cdr.detach();
    }
}
