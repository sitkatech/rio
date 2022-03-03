import {Injectable} from '@angular/core';
import {Alert} from '../models/alert';
import { AlertContext } from '../models/enums/alert-context.enum';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';

@Injectable({
    providedIn: 'root'
})
export class AlertService {

    constructor() {

    }

    public alerts: Alert[] = [];
    public alertSubject: BehaviorSubject<Alert[]> = new BehaviorSubject([]);

    pushAlert(alert: Alert): void {
        if (alert.uniqueCode ){
            if (this.alerts.some(x=>x.uniqueCode === alert.uniqueCode)){
                return; // don't push a duplicate alert if it has a unique token.
            }
        }
        this.alerts.push(alert);
        this.alertSubject.next(this.alerts);
    }

    removeAlert(alert: Alert): void {
        const index: number = this.alerts.indexOf(alert);
        this.alerts.splice(index, 1);
        this.alertSubject.next(this.alerts);
    }

    removeAlertsSubset(start, deleteCount) {
        this.alerts.splice(start, deleteCount);
        this.alertSubject.next(this.alerts);
    }

    getAlerts(): Alert[] {
        return this.alerts;
    }

    clearAlerts(): void {
        this.alerts = [];
        this.alertSubject.next(this.alerts);
    }   

    pushNotFoundUnauthorizedAlert(){
        this.pushAlert(new Alert("The page you are trying to access was not found, or you do not have permission to view it.", AlertContext.Info, true, AlertService.NOT_FOUND_UNAUTHORIZED));
        this.alertSubject.next(this.alerts);
    }


    public static NOT_FOUND_UNAUTHORIZED = "NotFoundUnauthorized";
    public static USERS_AWAITING_CONFIGURATION = "UsersAwaitingConfiguration";
}
