import {Injectable} from '@angular/core';
import {Alert} from '../models/alert';

@Injectable({
    providedIn: 'root'
})
export class AlertService {

    constructor() {

    }

    public alerts: Alert[] = [];

    pushAlert(alert: Alert): void {
        this.alerts.push(alert);
    }

    removeAlert(alert: Alert): void {
        const index: number = this.alerts.indexOf(alert);
        this.alerts.splice(index, 1);
    }

    getAlerts(): Alert[] {
        return this.alerts;
    }

    clearAlerts(): void {
        this.alerts = [];
    }

}
