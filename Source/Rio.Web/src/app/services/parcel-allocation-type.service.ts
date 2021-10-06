import { Injectable } from '@angular/core';
import { ApiService } from '../shared/services';
import { Observable } from 'rxjs';
import { ParcelAllocationTypeDto } from '../shared/models/parcel-allocation-type-dto';

@Injectable({
  providedIn: 'root'
})
export class ParcelAllocationTypeService {

  constructor(private apiService: ApiService) { }

  public getParcelAllocationTypes(): Observable<ParcelAllocationTypeDto[]>{
    return this.apiService.getFromApi('/water-types/');
  }

  public mergeParcelAllocationTypes(parcelAllocationTypes: ParcelAllocationTypeDto[]): Observable<any>{
    return this.apiService.putToApi('/parcel-allocation-types/', parcelAllocationTypes);
  }
}
