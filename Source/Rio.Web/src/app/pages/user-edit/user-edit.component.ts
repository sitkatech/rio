import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { FormArray, FormControl, FormGroup, Validators } from "@angular/forms";
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { RoleService } from 'src/app/services/role/role.service';
import { UserDto } from 'src/app/shared/models';
import { RoleDto } from 'src/app/shared/models/role/role-dto';
import { forkJoin } from 'rxjs';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { UserUpdateDto } from 'src/app/shared/models/user/user-update-dto';


@Component({
  selector: 'rio-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user-edit.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserEditComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public userID: number;
  public user: UserDto;
  public model: UserUpdateDto;
  public roles: Array<RoleDto>;
  public isLoadingSubmit: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authenticationService: AuthenticationService,
    private userService: UserService,
    private roleService: RoleService,
    private cdr: ChangeDetectorRef,
    private alertService: AlertService
  ) {
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;

      if (!this.authenticationService.isUserAnAdministrator(this.currentUser)) {
        this.router.navigateByUrl("/not-found")
          .then();
        return;
      }

      this.userID = parseInt(this.route.snapshot.paramMap.get("id"));

      forkJoin(
        this.userService.getUserFromUserID(this.userID),
        this.roleService.getRoles()
      ).subscribe(([user, roles]) => {
        this.user = user instanceof Array
          ? null
          : user as UserDto;

        this.roles = roles.sort((a: RoleDto, b: RoleDto) => {
          if (a.RoleDisplayName > b.RoleDisplayName)
            return 1;
          if (a.RoleDisplayName < b.RoleDisplayName)
            return -1;
          return 0;
        });

        this.model = new UserUpdateDto();
        this.model.RoleID = user.Role.RoleID;

        this.cdr.detectChanges();
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  onSubmit(editUserForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;

    this.userService.updateUser(this.userID, this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        // if (response instanceof Array) {
        //     this.alertService.pushAlert(new Alert("The form could not be submitted due to errors. Please correct the errors and try again.", AlertContext.Danger));
        //     this.formErrors = response;
        //     return;
        // }
        this.router.navigateByUrl("/users/" + this.userID).then(x => {
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
}