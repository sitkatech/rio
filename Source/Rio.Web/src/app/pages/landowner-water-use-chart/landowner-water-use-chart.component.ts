import { Component, OnInit, ViewEncapsulation, Input } from '@angular/core';

import { ColorHelper, SeriesHorizontal } from '@swimlane/ngx-charts';

import { single } from './data';
import { multi } from './data';

import {MultiSeriesEntry} from 'src/app/shared/models/multi-series';

@Component({
  selector: 'rio-landowner-water-use-chart',
  templateUrl: './landowner-water-use-chart.component.html',
  styleUrls: ['./landowner-water-use-chart.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LandownerWaterUseChartComponent implements OnInit {

  @Input() waterUsageData: MultiSeriesEntry[];

  public activeEntries: any[];
  public chartData: MultiSeriesEntry[];
  public chartNames: string[];
  public colors: ColorHelper;
  public colorScheme = { domain: ['#0000FF', '#008000'] }; // Custom color scheme in hex

  single: any[];
  multi: any[];

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
    Object.assign(this, { single });
    Object.assign(this, { multi });
  }

  ngOnInit() {
     // Get chartNames

    this.chartData = multi;
    //  this.chartNames = this.chartData.map((d: any) => d.name);
    //  console.log(this.chartNames);
    this.chartNames = ["Parcel 249 20 200", "Parcel 249 20 201"]
     // Convert hex colors to ColorHelper for consumption by legend
     this.colors = new ColorHelper(this.colorScheme, 'ordinal', this.chartNames, this.colorScheme);
  }

  public legendLabelActivate(item: any): void {
    this.activeEntries = [item];
  }

  public legendLabelDeactivate(item: any): void {
    this.activeEntries = [];
  }

}
