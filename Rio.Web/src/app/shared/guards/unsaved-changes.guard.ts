import { Injectable } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRouteSnapshot, CanDeactivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { ConfirmService } from '../services/confirm.service';

export interface IDeactivateComponent {
  canExit: () => Observable<boolean> | Promise<boolean> | boolean;
  onExit: () => Observable<void> | Promise<void> | void;
}

@Injectable({
  providedIn: 'root'
})
export class UnsavedChangesGuard implements CanDeactivate<IDeactivateComponent> {

  constructor(private confirmService: ConfirmService,
    private location: Location,
    private router: Router) { }

  canDeactivate(
    component: IDeactivateComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    
    //the system is forcing a redirect, so don't wait 
    if (nextState?.root.queryParams?.forcedRedirect) {
      return true;
    }
    
    if (component.canExit && !component.canExit()) {
      return this.confirmService.confirm({ buttonClassYes: 'btn-danger', modalSize: 'md', title: 'Warning: Unsaved Changes', message: 'You have unsaved changes on this page.  Are you sure you want to leave?', buttonTextYes: "Proceed", buttonTextNo: "Cancel" })
        .then(x => {
          if (component.onExit) {
            component.onExit();
          }

          if (!x) {
            //Even if we return false, Angular tells us that our last location on the stack is the URL we would've navigated to
            //So we need to update it ourselves
            this.location.go(this.location.path());
            this.router.navigate([currentState.url], { skipLocationChange: true });
          }
          return x;
        });
    }

    return true;
  }

}
