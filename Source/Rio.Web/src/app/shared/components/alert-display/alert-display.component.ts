import {Component, OnDestroy, OnInit} from '@angular/core';
import {AlertService} from '../../services/alert.service';
import {Alert} from '../../models/alert';

@Component({
    selector: 'app-alert-display',
    templateUrl: './alert-display.component.html',
    styleUrls: ['./alert-display.component.css']
})
export class AlertDisplayComponent implements OnInit, OnDestroy {

    public alerts: Alert[] = [];

    constructor(
        private alertService: AlertService,
    ) {
    }

    public ngOnInit(): void {
        this.alerts = this.alertService.getAlerts();
    }

    public ngOnDestroy(): void {
        this.alertService.clearAlerts();
    }

    public closeAlert(alert: Alert) {
        this.alertService.removeAlert(alert);
    }

}
