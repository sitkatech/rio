import { Injectable } from '@angular/core';
import { ApiService } from '.';
import { Observable } from 'rxjs';
import { CustomRichTextDto } from '../models/custom-rich-text-dto';

@Injectable({
  providedIn: 'root'
})
export class CustomRichTextService {
  constructor(private apiService: ApiService) { }

  public getCustomRichText(customRichTextTypeID: number): Observable<CustomRichTextDto> {
    return this.apiService.getFromApi(`/customRichText/${customRichTextTypeID}`)
  }
  
  public updateCustomRichText(customRichTextTypeID: number, updateDto: CustomRichTextDto): Observable<CustomRichTextDto> {
    return this.apiService.putToApi(`customRichText/${customRichTextTypeID}`, updateDto);
  }
}
