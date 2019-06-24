import { Injectable } from '@angular/core';
import { ApiService } from '..';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StatusService {

  constructor(private apiRoute: ApiService) {

  }

  getStatus(): Observable<any> {

      var status = this.apiRoute.getFromApi(`/service/status`);

      return status;
  }
}
