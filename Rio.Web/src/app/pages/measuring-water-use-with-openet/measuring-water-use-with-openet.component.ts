import { Component, OnInit } from '@angular/core';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-water-use-measurement',
  templateUrl: './measuring-water-use-with-openet.component.html',
  styleUrls: ['./measuring-water-use-with-openet.component.scss']
})
export class WaterUseMeasurementComponent implements OnInit {
  public richTextTypeID : number = CustomRichTextType.MeasuringWaterUse;
  constructor() { }

  ngOnInit() {
  }

}
