import { Component, OnInit } from '@angular/core';

import { single } from './data';
import { multi } from './data';

@Component({
  selector: 'rio-landowner-water-use-chart',
  templateUrl: './landowner-water-use-chart.component.html',
  styleUrls: ['./landowner-water-use-chart.component.scss']
})
export class LandownerWaterUseChartComponent implements OnInit {

  single: any[];
  multi: any[];

  view: any[] = [700, 400];

  // options
  showXAxis = true;
  showYAxis = true;
  gradient = false;
  showLegend = true;
  showXAxisLabel = true;
  xAxisLabel = 'Volume (AF)';
  showYAxisLabel = true;
  yAxisLabel = 'Population';

  colorScheme = {
    domain: ['#5AA454', '#A10A28', '#C7B42C', '#AAAAAA']
  };


  constructor() { }

  ngOnInit() {
    Object.assign(this, { single })
    Object.assign(this, { multi })
  }

}
