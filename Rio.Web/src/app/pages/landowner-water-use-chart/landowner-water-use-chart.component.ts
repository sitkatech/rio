import { Component, OnInit, ViewEncapsulation, Input } from '@angular/core';

import { Color, ColorHelper, ScaleType } from '@swimlane/ngx-charts';

import { MultiSeriesEntry } from "src/app/shared/models/series-entry";

import * as palette from 'google-palette';


@Component({
  selector: 'rio-landowner-water-use-chart',
  templateUrl: './landowner-water-use-chart.component.html',
  styleUrls: ['./landowner-water-use-chart.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LandownerWaterUseChartComponent implements OnInit {

  @Input() waterUsageData: MultiSeriesEntry[];
  @Input() parcelNumbers: string[];

  public activeEntries: any[];
  public chartData: MultiSeriesEntry[];

  public colors: ColorHelper;
  public colorScheme: Color = { name: "parcelColors", domain: [], selectable: true, group: ScaleType.Ordinal};

  view: any[] = [700, 400];

  constructor() {
  }

  ngOnInit() {    
    this.buildColorScheme();
  }

  public legendLabelActivate(item: any): void {
    this.activeEntries = [item];
  }

  public legendLabelDeactivate(item: any): void {
    this.activeEntries = [];
  }

  public buildColorScheme(){
    let colorList: string[];

    const numOfParcels = this.parcelNumbers.length;
    colorList = palette('mpn65', numOfParcels).map(x =>`#${x}`);
    
    this.colorScheme = { name: "parcelColors", domain: colorList, selectable: true, group: ScaleType.Ordinal};
    this.colors = new ColorHelper(this.colorScheme, ScaleType.Ordinal, this.parcelNumbers, null);
  }

}
