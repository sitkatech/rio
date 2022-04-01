import { Component, OnInit } from '@angular/core';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';

@Component({
  selector: 'rio-water-use-measurement',
  templateUrl: './measuring-water-use-with-openet.component.html',
  styleUrls: ['./measuring-water-use-with-openet.component.scss']
})
export class WaterUseMeasurementComponent implements OnInit {
  public richTextTypeID : number = CustomRichTextTypeEnum.MeasuringWaterUse;
  constructor() { }

  ngOnInit() {
  }

}
