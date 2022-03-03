import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

export interface Busy {
  show: boolean;
  text?: string;
}

@Injectable({
  providedIn: 'root'
})
export class BusyService {

  private busySubject: Subject<Busy>;
  private startTime: number;
  private timeoutInterval: any;

  constructor() {
    this.busySubject = new Subject();
  }

  getBusy$(): Observable<Busy> {
    return this.busySubject.asObservable();
  }

  setBusy(show: boolean, text?: string) {
    this.startTime = performance.now();

    setTimeout(() => {
      if (show) {
        this.setTimeoutIntervalCheck();
      } else {
        this.clearTimeoutInterval();
      }

      this.busySubject.next({
        show: show,
        text: text || '',
      });
    });
  }

  private setTimeoutIntervalCheck() {
    if (!this.timeoutInterval) {
      this.timeoutInterval = setInterval(() => {
        if (performance.now() - this.startTime > 30 * 1000) {
          this.clearTimeoutInterval();
          this.busySubject.next({ show: false });
          // this.messageService.add({
          //   severity: 'error',
          //   summary: 'Timeout',
          //   detail: `Oops! Something went wrong and the operation timed-out.<br>
          //            You may want to retry the operation...`
          // });
        }
      }, 5 * 1000);
    }
  }

  private clearTimeoutInterval() {
    if (this.timeoutInterval) {
      clearInterval(this.timeoutInterval);
      this.timeoutInterval = 0;
    }
  }
}
