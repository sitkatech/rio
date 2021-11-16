import {ChangeDetectorRef, Component, OnDestroy, OnInit} from '@angular/core';
import {AlertService} from '../../services/alert.service';
import {Alert} from '../../models/alert';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-alert-display',
    templateUrl: './alert-display.component.html',
    styleUrls: ['./alert-display.component.css']
})
export class AlertDisplayComponent implements OnInit, OnDestroy {

    public alerts: Alert[] = [];
    private alertSubscription: Subscription;

    constructor(
        private alertService: AlertService,
        private cdr: ChangeDetectorRef,
    ) {
    }

    public ngOnInit(): void {
        this.alertSubscription = this.alertService.alertSubject.asObservable().subscribe(alerts=>{
            this.alerts = alerts;
            this.cdr.detectChanges();
        })
    }

    public ngOnDestroy(): void {
        this.alerts = null;
        this.alertSubscription.unsubscribe();
        this.alertService.clearAlerts();
    }

    public closeAlert(alert: Alert) {
        this.alertService.removeAlert(alert);
    }
}
