import { Component, OnInit } from '@angular/core';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';

@Component({
  selector: 'rio-glossary',
  templateUrl: './glossary.component.html',
  styleUrls: ['./glossary.component.scss']
})
export class GlossaryComponent implements OnInit {
  waterTypes: WaterTypeDto[];

  constructor(
    private waterTypeService: WaterTypeService
  ) { }

  ngOnInit() {
    this.waterTypeService.getWaterTypes().subscribe(x=>{
      this.waterTypes = x;
    })
  }

}
