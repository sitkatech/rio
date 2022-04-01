import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { Injectable } from '@angular/core';
import { AlertService } from '../../services/alert.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { RoleEnum } from 'src/app/shared/generated/enum/role-enum';

@Injectable({
  providedIn: 'root'
})
export class ManagerOnlyGuard implements CanActivate {
  constructor(private router: Router, private alertService: AlertService, private authenticationService: AuthenticationService) {
  }
  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    if (!this.authenticationService.isCurrentUserNullOrUndefined()) {
      if (this.authenticationService.isCurrentUserAnAdministrator()) {
        return true;
      } else {
        this.alertService.pushNotFoundUnauthorizedAlert();
        this.router.navigate(["/"]);
        return false;
      }
    }

    return this.authenticationService.getCurrentUser()
      .pipe(
        map(x => {
          if (x.Role.RoleID == RoleEnum.Admin) {
            return true;
          } else {
            this.alertService.pushNotFoundUnauthorizedAlert();
            this.router.navigate(["/"]);
            return false;
          }
        })
      );
  }
}
