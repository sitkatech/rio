import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from "rxjs";
import { Injectable } from '@angular/core';
import { AlertService } from '../../services/alert.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
@Injectable({
  providedIn: 'root'
})
export class ManagerOnlyGuard implements CanActivate {
  // for locking down routes that don't make an unauthorized API calls but still aren't an intended part of the UX
  constructor(private router: Router, private alertService: AlertService, private authenticationService: AuthenticationService) {
  }
  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    if (this.authenticationService.isCurrentUserAnAdministrator()) {
      return true;
    }
    else {
      this.router.navigate(["/"]);
      this.alertService.pushNotFoundUnauthorizedAlert();
    }
  }
}
