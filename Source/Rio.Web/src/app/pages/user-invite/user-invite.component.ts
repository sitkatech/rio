import { Component, OnInit, Output, Input, EventEmitter, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { SystemRoleService } from 'src/app/services/role/role.service';
import { OAuthService } from 'angular-oauth2-oidc';
import { ValidationError } from "src/app/shared/models/validation-error";
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { forkJoin } from "rxjs";



@Component({
    selector: 'rio-user-invite',
    templateUrl: './user-invite.component.html',
    styleUrls: ['./user-invite.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserInviteComponent implements OnInit {

    public roles: any = [];
    public model: any = {};
    public isLoadingSubmit: boolean = false;

    constructor(private cdr: ChangeDetectorRef, private router: Router, private userService: UserService, private roleService: SystemRoleService, private authenticationService: OAuthService, private alertService: AlertService) { }

    ngOnInit(): void {
        this.roleService.getRoles().subscribe(result => {
            this.roles = result;
            this.cdr.detectChanges();
        });
    }

    canInviteUser(): boolean {
        return this.model.FirstName && this.model.LastName && this.model.RoleID && this.model.Email && this.model.Email.indexOf("@") != -1;
    }

    onSubmit(inviteUserForm: HTMLFormElement): void {
        this.isLoadingSubmit = true;

        this.userService.inviteUser(this.model)
            .subscribe(response => {
                this.isLoadingSubmit = false;
                // if (response instanceof Array) {
                //     this.alertService.pushAlert(new Alert("The form could not be submitted due to errors. Please correct the errors and try again.", AlertContext.Danger));
                //     this.formErrors = response;
                //     return;
                // }
                inviteUserForm.reset();
                this.router.navigateByUrl("/users").then(x => {
                    this.alertService.pushAlert(new Alert("Your request was successfully submitted.", AlertContext.Success));
                });
            }
                ,
                error => {
                    this.isLoadingSubmit = false;
                    this.cdr.detectChanges();
                }
            );
    }

    public currentUserIsAdmin(): boolean {
        return this.authenticationService.hasValidAccessToken();
    }
}
