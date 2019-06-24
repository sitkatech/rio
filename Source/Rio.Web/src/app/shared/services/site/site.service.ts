import { Injectable } from '@angular/core';
import { ApiService } from '../api/api.service';
import { Site } from '../../models';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs/internal/Observable';

@Injectable({
  providedIn: 'root'
})
export class SiteService {

  constructor(private api: ApiService) { }

  getSites(): Observable<Site[]> {
    const relativeRoute = `/sites`;
    const request = this.api.getFromApi(relativeRoute)
    .pipe(
      map((items: any[]) => {
        const result = [];
        for (const item of items || []) {
          result.push(new Site(item));
        }
        return result;
      })
    );
    return request;
  }

  getSite(siteID: number) {
    const relativeRoute = `/sites/${siteID}`;
    const request = this.api.getFromApi(relativeRoute)
    .pipe(
      map((item: any) => {
        return new Site(item);
      })
    );
    return request;
  }
}
