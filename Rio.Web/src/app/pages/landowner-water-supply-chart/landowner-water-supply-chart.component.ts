import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { SeriesEntry, MultiSeriesEntry } from "src/app/shared/models/series-entry";
import { environment } from 'src/environments/environment';
@Component({
  selector: 'rio-landowner-water-supply-chart',
  templateUrl: './landowner-water-supply-chart.component.html',
  styleUrls: ['./landowner-water-supply-chart.component.scss']
})
export class LandownerWaterSupplyChartComponent implements OnInit {
  @Input() annualWaterSupplySeries?: any;
  @Input() currentCumulativeWaterUsage: any;
  @Input() historicCumulativeWaterUsage: any;
  @Input() waterSupplyLabel: string = environment.allowTrading ? "Annual Water Supply (Water Supply +/- Trades)" : "Annual Supply"

  fake: MultiSeriesEntry[] = [];

  view: any[] = [755, 400];

  @Input() yDomain;

  lineChartScheme = {
    name: 'coolthree',
    selectable: true,
    group: 'Ordinal',
    // gray, blue
    domain: ['#636363', '#0400d6'] 
  };

  comboBarScheme = {
    name: 'singleLightBlue',
    selectable: true,
    group: 'Ordinal',
    domain: ['#0400d6']
  };
  seriesDomain: string[];

  constructor() { }
  ngOnInit() {
    this.seriesDomain = ["Cumulative Monthly Usage", "Average Usage (All Years)"];

    if (environment.includeWaterSupply) {
      this.seriesDomain.push(this.waterSupplyLabel);
      // add red to line chart color scheme if displaying Water Supply line
      this.lineChartScheme.domain.splice(1, 0, '#ff1100');
    }
  }
}
