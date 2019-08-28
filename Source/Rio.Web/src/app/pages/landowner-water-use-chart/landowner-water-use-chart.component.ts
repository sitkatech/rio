import { Component, OnInit, ViewEncapsulation, Input } from '@angular/core';

import { ColorHelper } from '@swimlane/ngx-charts';

import {MultiSeriesEntry} from 'src/app/shared/models/water-usage-dto';

import * as ColorScheme from 'color-scheme';

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
  public colorScheme: {domain: string[]};

  view: any[] = [700, 400];

  // options
  showXAxis = true;
  showYAxis = true;
  gradient = false;
  showLegend = true;
  showXAxisLabel = true;
  yAxisLabel = 'Volume (AF)';
  showYAxisLabel = true;
  xAxisLabel = 'Month';

  constructor() {
  }

  ngOnInit() {

    let scheme = new ColorScheme;
    scheme.from_hue(21).scheme('triade').variation('soft');
    let colorList = (scheme.colors() as string[]).slice(0, this.parcelNumbers.length - 1).map(x=>`#${x}`);
    console.log(scheme.colors());
    console.log(this.parcelNumbers);
    
    this.colorScheme = {domain: colorList };
    this.colors = new ColorHelper(this.colorScheme, 'ordinal', this.parcelNumbers, this.colorScheme);
    throw Error("had to do it to ya");
  }

  public legendLabelActivate(item: any): void {
    this.activeEntries = [item];
  }

  public legendLabelDeactivate(item: any): void {
    this.activeEntries = [];
  }

}
