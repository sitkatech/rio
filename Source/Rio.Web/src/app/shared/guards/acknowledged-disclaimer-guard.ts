import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Observable } from 'rxjs';
import { map } from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class AcknowledgedDisclaimerGuard implements CanActivate {

  constructor(private router: Router, private authenticationService: AuthenticationService) {
  }

  canActivate(
    next: ActivatedRouteSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      if(!this.authenticationService.isCurrentUserNullOrUndefined()) {
        if (!this.authenticationService.hasCurrentUserAcknowledgedDisclaimer()) {
          this.router.navigate(["/disclaimer/true"], {
            queryParams: {
              route: next.routeConfig.path,
              queryParams : JSON.stringify(next.queryParams)
            }
          });
          return false;
        }
        else {
          return true;
        }
      }

      return this.authenticationService.currentUserSetObservable
      .pipe(
        map(x => {
          if (x.DisclaimerAcknowledgedDate != null) {
            return true;
          } else {
            this.router.navigate(["/disclaimer/true"], {
              queryParams: {
                route: next.routeConfig.path,
                queryParams : JSON.stringify(next.queryParams)
              }
            });
            return false;
          }
        })
      );
  }

}