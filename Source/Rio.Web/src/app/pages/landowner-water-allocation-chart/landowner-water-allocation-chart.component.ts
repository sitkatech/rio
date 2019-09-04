import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { SeriesEntry, MultiSeriesEntry } from 'src/app/shared/models/water-usage-dto';
@Component({
  selector: 'rio-landowner-water-allocation-chart',
  templateUrl: './landowner-water-allocation-chart.component.html',
  styleUrls: ['./landowner-water-allocation-chart.component.scss']
})
export class LandownerWaterAllocationChartComponent implements OnInit {
  @Input() annualAllocationSeries: any;
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
  seriesDomain = ["Historic", "Current", "Annual Allocation"];

  yDomain = [0, 2000];

  lineChartScheme = {
    name: 'coolthree',
    selectable: true,
    group: 'Ordinal',
    domain: ['#01579b', '#a8385d', '#7aa3e5'] //7aa3e5
  };

  comboBarScheme = {
    name: 'singleLightBlue',
    selectable: true,
    group: 'Ordinal',
    domain: ['#7aa3e5']
  };

  constructor() { }
  ngOnInit() {}
}
