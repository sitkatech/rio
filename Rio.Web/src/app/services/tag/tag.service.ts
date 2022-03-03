import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { TagDto } from 'src/app/shared/generated/model/tag-dto';
import { TagBulkSetUpsertDto } from 'src/app/shared/generated/model/tag-bulk-set-upsert-dto';

@Injectable({
  providedIn: 'root'
})
export class TagService {

  constructor(private apiService: ApiService, private httpClient: HttpClient) { }

  getAllTags(): Observable<Array<TagDto>> {
    let route = `/tags`;
    return this.apiService.getFromApi(route);
  }

  getTagByID(tagID: number): Observable<TagDto> {
    let route = `/tags/${tagID}`;
    return this.apiService.getFromApi(route);
  }

  getTagsByParcelID(parcelID: number): Observable<Array<TagDto>> {
    let route = `/tags/listByParcelID/${parcelID}`;
    return this.apiService.getFromApi(route);
  }

  createTag(tagDto: TagDto) {
    let route = `/tags/create`;
    return this.apiService.postToApi(route, tagDto);
  }

  updateTag(tagDto: TagDto): Observable<TagDto> {
    let route = `/tags/update`;
    return this.apiService.putToApi(route, tagDto);
  }

  deleteTag(tagID: number) {
    let route = `/tags/${tagID}`;
    return this.apiService.deleteToApi(route);
  }

  tagParcel(parcelID: number, tagDto: TagDto) {
    let route = `tags/tagParcel/${parcelID}`;
    return this.apiService.postToApi(route, tagDto);
  }

  bulkTagParcel(tagBulkSetUpsertDto: TagBulkSetUpsertDto): Observable<number> {
    let route = `tags/bulkTagParcels`;
    return this.apiService.postToApi(route, tagBulkSetUpsertDto);
  }

  removeTagFromParcel(tagID: number, parcelID: number) {
    let route = `tags/${tagID}/removeTagFromParcel/${parcelID}`;
    return this.apiService.deleteToApi(route);
  }
}
