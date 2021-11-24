import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { SeriesEntry, MultiSeriesEntry } from "src/app/shared/models/series-entry";
import { environment } from 'src/environments/environment';
@Component({
  selector: 'rio-landowner-water-allocation-chart',
  templateUrl: './landowner-water-allocation-chart.component.html',
  styleUrls: ['./landowner-water-allocation-chart.component.scss']
})
export class LandownerWaterAllocationChartComponent implements OnInit {
  @Input() annualAllocationSeries: any;
  @Input() currentCumulativeWaterUsage: any;
  @Input() historicCumulativeWaterUsage: any;
  @Input() allocationLabel: string = environment.allowTrading ? "Annual Supply (Allocation +/- Trades)" : "Annual Supply"

  fake: MultiSeriesEntry[] = [];

  view: any[] = [755, 400];

  @Input() yDomain;

  lineChartScheme = {
    name: 'coolthree',
    selectable: true,
    group: 'Ordinal',
    domain: ['#FFA600', '#A05195', '#03583F'] 
  };

  comboBarScheme = {
    name: 'singleLightBlue',
    selectable: true,
    group: 'Ordinal',
    domain: ['#03583F']
  };
  seriesDomain: string[];

  constructor() { }
  ngOnInit() {
    this.seriesDomain = ["Cumulative Monthly Usage", "Average Usage (All Years)", this.allocationLabel];
  }
}
