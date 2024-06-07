import { Injectable } from '@angular/core';
import { ApiService } from '../shared/services';
import { Observable } from 'rxjs';
import { SystemInfoDto } from '../shared/generated/model/models';

@Injectable({
  providedIn: 'root'
})
export class SystemInfoService {

  constructor(private apiService: ApiService) { }

  public getSystemInfo(): Observable<SystemInfoDto>{
    return this.apiService.getFromApi('/');
  }

}
