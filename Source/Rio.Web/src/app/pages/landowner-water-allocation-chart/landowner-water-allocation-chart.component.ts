import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { SeriesEntry, MultiSeriesEntry } from 'src/app/shared/models/water-usage-dto';
import { barChart } from '../combo-chart-test/combo-chart-test.component';

@Component({
  selector: 'rio-landowner-water-allocation-chart',
  templateUrl: './landowner-water-allocation-chart.component.html',
  styleUrls: ['./landowner-water-allocation-chart.component.scss']
})
export class LandownerWaterAllocationChartComponent implements OnInit {

  
  @Input() annualAllocation: number;

  @Input() currentCumulativeWaterUsage: any;
  @Input() historicCumulativeWaterUsage: any;

  fake: MultiSeriesEntry[] = [];

  view: any[] = [700, 400];
  showXAxis = true;
  showYAxis = true;
  gradient = false;
  showLegend = true;
  legendTitle = 'Legend';
  legendPosition = 'right';
  showXAxisLabel = true;
  showYAxisLabel = true;
  yAxisLabel = 'Volume (acre-feet)';
  xAxisLabel = 'Month';
  showGridLines = true;
  innerPadding = '10%';
  animations: boolean = true;
  seriesDomain = ["Historic", "Current"];
yDomain = [0, 2000];

  lineChartScheme = {
    name: 'coolthree',
    selectable: true,
    group: 'Ordinal',
    domain: ['#01579b', '#7aa3e5', '#a8385d', '#00bfa5']
  };

  comboBarScheme = {
    name: 'singleLightBlue',
    selectable: true,
    group: 'Ordinal',
    domain: ['#7aa3e5']
  };

  constructor() { }

  ngOnInit() {
  }
}
