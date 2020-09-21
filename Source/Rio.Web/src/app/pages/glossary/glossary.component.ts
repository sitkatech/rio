import { Component, OnInit } from '@angular/core';
import { ParcelAllocationTypeService } from 'src/app/services/parcel-allocation-type.service';
import { ParcelAllocationTypeDto } from 'src/app/shared/models/parcel-allocation-type-dto';

@Component({
  selector: 'rio-glossary',
  templateUrl: './glossary.component.html',
  styleUrls: ['./glossary.component.scss']
})
export class GlossaryComponent implements OnInit {
  parcelAllocationTypes: ParcelAllocationTypeDto[];

  constructor(
    private parcelAllocationTypeService: ParcelAllocationTypeService
  ) { }

  ngOnInit() {
    this.parcelAllocationTypeService.getParcelAllocationTypes().subscribe(x=>{
      this.parcelAllocationTypes = x;
    })
  }

}
