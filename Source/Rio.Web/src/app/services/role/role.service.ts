import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class RoleService {

    constructor(private apiService: ApiService) { }
        
    getRoles(): Observable<any[]> {
        let route = `/roles`;
        return this.apiService.getFromApi(route);
    }
}
