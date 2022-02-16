import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { TagDto } from 'src/app/shared/generated/model/tag-dto';

@Injectable({
  providedIn: 'root'
})
export class TagService {

  constructor(private apiService: ApiService, private httpClient: HttpClient) { }

  getAllTags(): Observable<Array<TagDto>> {
    let route = `/tags`;
    return this.apiService.getFromApi(route);
  }

  deleteTag(tagID) {
    let route = `tags/${tagID}`;
    return this.apiService.deleteToApi(route);
  }
}
