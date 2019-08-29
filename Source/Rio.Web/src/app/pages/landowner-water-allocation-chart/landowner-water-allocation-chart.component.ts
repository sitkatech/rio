import { Component, OnInit, Input } from '@angular/core';
import { SeriesEntry, MultiSeriesEntry } from 'src/app/shared/models/water-usage-dto';
import { barChart } from '../combo-chart-test/combo-chart-test.component';

@Component({
  selector: 'rio-landowner-water-allocation-chart',
  templateUrl: './landowner-water-allocation-chart.component.html',
  styleUrls: ['./landowner-water-allocation-chart.component.scss']
})
export class LandownerWaterAllocationChartComponent implements OnInit {
  @Input() annualAllocation: number;
  usageData: {
    Historic: MultiSeriesEntry[],
    Current: SeriesEntry[]
  }

  view = [500, 400];
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
    domain: ['#01579b']
  };

  showRightYAxisLabel: boolean = false;
  yAxisLabelRight: string = 'Utilization';
  constructor() { }

  ngOnInit() {
    this.usageData = {
      Historic: this.lineChartSeries,
      Current: this.barChart 
    };
  }

  lineChartSeries = [
    {
      name: 'Tablets',
      series: [
        {
          name: 'USA',
          value: 50
        },
        {
          value: 80,
          name: 'United Kingdom'
        },
        {
          value: 85,
          name: 'France'
        },
        {
          value: 90,
          name: 'Japan'
        },
        {
          value: 100,
          name: 'China'
        }
      ]
    },
    {
      name: 'Cell Phones',
      series: [
        {
          value: 10,
          name: 'USA'
        },
        {
          value: 20,
          name: 'United Kingdom'
        },
        {
          value: 30,
          name: 'France'
        },
        {
          value: 40,
          name: 'Japan'
        },
        {
          value: 10,
          name: 'China'
        }
      ]
    },
    {
      name: 'Computers',
      series: [
        {
          value: 2,
          name: 'USA',

        },
        {
          value: 4,
          name: 'United Kingdom'
        },
        {
          value: 20,
          name: 'France'
        },
        {
          value: 30,
          name: 'Japan'
        },
        {
          value: 35,
          name: 'China'
        }
      ]
    }
  ];

  barChart = [
    {
      name: 'USA',
      value: 50000
    },
    {
      name: 'United Kingdom',
      value: 30000
    },
    {
      name: 'France',
      value: 10000
    },
    {
      name: 'Japan',
      value: 5000
    },
    {
      name: 'China',
      value: 500
    }
  ];


}
