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

  view: any[] = [755, 400];
  
  seriesDomain = ["Historic", "Current", "Annual Allocation"];

  @Input() yDomain;

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
