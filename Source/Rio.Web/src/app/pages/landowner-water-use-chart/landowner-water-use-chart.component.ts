import { Component, OnInit, ViewEncapsulation, Input } from '@angular/core';

import { ColorHelper } from '@swimlane/ngx-charts';

import {MultiSeriesEntry} from 'src/app/shared/models/water-usage-dto';

import * as ColorScheme from 'color-scheme';
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
    let colorList: string[];

    const numOfParcels = this.parcelNumbers.length;

    //colorList = palette('mpn65', numOfParcels).map(x=>`#${x}`);

    // let scheme = new ColorScheme;
    
    // scheme.from_hue(21).scheme('triade').variation('soft');
    
    // let colorList = (scheme.colors() as string[]).slice(0, this.parcelNumbers.length - 1).map(x=>`#${x}`);
    // console.log(scheme.colors());
    // console.log(this.parcelNumbers);

    
    this.colorScheme = {domain: ["#ff0029", "#377eb8", "#66a61e", "#984ea3", "#00d2d5", "#ff7f00", "#af8d00", "#7f80cd", "#b3e900", "#c42e60", "#a65628", "#f781bf", "#8dd3c7", "#bebada", "#fb8072", "#80b1d3", "#fdb462", "#fccde5", "#bc80bd", "#ffed6f", "#c4eaff"] };
    this.colors = new ColorHelper(this.colorScheme, 'ordinal', this.parcelNumbers, this.colorScheme);
  }

  public legendLabelActivate(item: any): void {
    this.activeEntries = [item];
  }

  public legendLabelDeactivate(item: any): void {
    this.activeEntries = [];
  }

}
