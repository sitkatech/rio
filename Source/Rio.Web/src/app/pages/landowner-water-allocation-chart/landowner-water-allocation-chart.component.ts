import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { SeriesEntry, MultiSeriesEntry } from "src/app/shared/models/series-entry";
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

  view: any[] = [755, 400];
  
  seriesDomain = ["Historic", "Current", "Annual Allocation"];

  @Input() yDomain;

  lineChartScheme = {
    name: 'coolthree',
    selectable: true,
    group: 'Ordinal',
    domain: ['#636363', '#01943a', '#0400d6'] 
  };

  comboBarScheme = {
    name: 'singleLightBlue',
    selectable: true,
    group: 'Ordinal',
    domain: ['#0400d6']
  };

  constructor() { }
  ngOnInit() {}
}
