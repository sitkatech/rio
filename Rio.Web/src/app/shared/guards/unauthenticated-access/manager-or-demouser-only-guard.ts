import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { Injectable } from '@angular/core';
import { AlertService } from '../../services/alert.service';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Injectable({
  providedIn: 'root'
})
export class ManagerOrDemoUserOnlyGuard implements CanActivate {
  constructor(private router: Router, private alertService: AlertService, private authenticationService: AuthenticationService) {
  }
  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    if (!this.authenticationService.isCurrentUserNullOrUndefined()) {
      if (this.authenticationService.isCurrentUserADemoUserOrAdministrator()) {
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
          if (this.authenticationService.isUserADemoUserOrAdministrator(x)) {
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
