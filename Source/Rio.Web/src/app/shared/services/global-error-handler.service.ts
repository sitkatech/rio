import { Injectable, Injector } from '@angular/core';
import { BusyService } from '.';
import { Alert } from '../models/alert';
import { AlertService } from './alert.service';

@Injectable({
  providedIn: 'root'
})
export class GlobalErrorHandlerService {

  private busyService: BusyService;
  private alertService: AlertService;

  constructor(
    private injector: Injector,
  ) {
    this.busyService = this.injector.get(BusyService);
    this.alertService = this.injector.get(AlertService);
  }

  handleError(error: any) {
    if (
      error &&
      error.status !== 401 && // Unauthorized
      error.status !== 403 && // Forbidden
      error.status !== 404 && // Not Found (can easily happen when looking for a unexisting .po file)
      (error.message || '').indexOf('ViewDestroyedError: Attempt to use a destroyed view: detectChanges') < 0 && // issue in the ngx-loading package...waiting for it to be updated.
      (error.message || '').indexOf('ExpressionChangedAfterItHasBeenCheckedError') < 0 && // this only happens in dev angular build - I'm sure
      (error.message || '').indexOf('Loading chunk') < 0 && // also ignore loading chunk errors as they're handled in app.component NavigationError event
      (error.message || '').indexOf('<path> attribute d: Expected number,') < 0 // attrTween.js error related to charts
    ) {
      // IE Bug
      if ((error.message || '').indexOf('available to complete this operation.') >= 0) {
        this.alertService.pushAlert(
          new Alert(`Internet Explorer Error: ${error.message}`)
        );
      }

      console.error(error);
      if ((window as any).appInsights) {
        (window as any).appInsights.trackException({
          exception: error.originalError || error
        });
      }
    } else if (error) {
      console.warn(error);
      this.busyService.setBusy(false);
    }
  }
}