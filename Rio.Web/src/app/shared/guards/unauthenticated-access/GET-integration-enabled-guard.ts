import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, from, of } from "rxjs";
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { AlertService } from '../../services/alert.service';
import { Alert } from '../../models/alert';

@Injectable({
  providedIn: 'root'
})
export class GETIntegrationEnabledGuard implements CanActivate {
  constructor(private router: Router, private alertService: AlertService) {
  }
  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    if (!environment.enabledGETIntegration) {
      this.router.navigate(["/"]);
      this.alertService.pushAlert(new Alert("This feature is unavailable."));
    }
    return environment.enabledGETIntegration;
  }
}
