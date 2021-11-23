import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { RoleService } from 'src/app/services/role/role.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { environment } from 'src/environments/environment';
import { RoleDto } from 'src/app/shared/generated/model/role-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { UserInviteDto } from 'src/app/shared/generated/model/user-invite-dto';



@Component({
    selector: 'rio-user-invite',
    templateUrl: './user-invite.component.html',
    styleUrls: ['./user-invite.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserInviteComponent implements OnInit, OnDestroy {
    private watchUserChangeSubscription: any;
    private currentUser: UserDto;

    public roles: Array<RoleDto>;
    public model: UserInviteDto;
    public isLoadingSubmit: boolean = false;

    constructor(private cdr: ChangeDetectorRef, 
        private route: ActivatedRoute,
        private router: Router, private userService: UserService, private roleService: RoleService, private authenticationService: AuthenticationService, private alertService: AlertService) { }

    ngOnInit(): void {
        this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
            this.currentUser = currentUser;
            this.roleService.getRoles().subscribe(result => {
                this.roles = result;
                this.cdr.detectChanges();
            });

            this.model = new UserInviteDto();

            const userID = parseInt(this.route.snapshot.paramMap.get("userID"));
            if (userID) {
                forkJoin(
                    this.userService.getUserFromUserID(userID)
                ).subscribe(([user]) => {
                    if(user.UserGuid === null)
                    {
                        let userToInvite = user instanceof Array
                            ? null
                            : user as UserDto;
                        this.model.Email = userToInvite.Email;
                        this.model.FirstName = userToInvite.FirstName;
                        this.model.LastName = userToInvite.LastName;
                        this.model.RoleID = userToInvite.Role.RoleID;
                        this.cdr.detectChanges();
                    }
                });
            }
        });
    }

    ngOnDestroy() {
        this.watchUserChangeSubscription.unsubscribe();
        this.authenticationService.dispose();
        this.cdr.detach();
    }

    canInviteUser(): boolean {
        return this.model.FirstName && this.model.LastName && this.model.RoleID && this.model.Email && this.model.Email.indexOf("@") != -1;
    }

    onSubmit(inviteUserForm: HTMLFormElement): void {
        this.isLoadingSubmit = true;

        this.userService.inviteUser(this.model)
            .subscribe(response => {
                this.isLoadingSubmit = false;
                inviteUserForm.reset();
                this.router.navigateByUrl(`/users/${response.UserID}`).then(x => {
                    this.alertService.pushAlert(new Alert("The user invite was successful.", AlertContext.Success));
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
        return this.authenticationService.isUserAnAdministrator(this.currentUser);
    }

    public platformShortName():string{
        return environment.platformShortName;
    }
}
