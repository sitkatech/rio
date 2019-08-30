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
  @Input() currentCumulativeWaterUsage: SeriesEntry[];

  @Input() historicCumulativeWaterUsage: MultiSeriesEntry[];

  fake: MultiSeriesEntry[] = [];

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
    this.fake = this.lineChartSeries;
    
  }

  public lineChartSeries = [
    {
        name: "Historic",
        series: [
            {
                name: "January",
                value: 25.1508
            },
            {
                name: "February",
                value: 66.7681
            },
            {
                name: "March",
                value: 151.13046666666668
            },
            {
                name: "April",
                value: 299.2504666666667
            },
            {
                name: "May",
                value: 479.4572
            },
            {
                name: "June",
                value: 697.6318
            },
            {
                name: "July",
                value: 937.4668333333333
            },
            {
                name: "August",
                value: 1129.9448
            },
            {
                name: "September",
                value: 1255.4465333333333
            },
            {
                name: "October",
                value: 1332.2960333333333
            },
            {
                name: "November",
                value: 1372.5892666666666
            },
            {
                name: "December",
                value: 1400.199
            }
        ]
    }
  ];

  public barChart = [
    {
      name: "January", value: 12.9587
    },
    {
      name: "February", value: 44.1887
    },
    {
      name: "March", value: 128.5509
    },
    {
      name: "April", value: 256.7861
    },
    {
      name: "May", value: 427.8011
    },
    {
      name: "June", value: 645.4448
    },
    {
      name: "July", value: 874.671
    },
    {
      name: "August", value: 1048.7093
    },
    {
      name: "September", value: 1159.8971
    },
    {
      name: "October", value: 1220.4909
    },
    {
      name: "November", value: 1245.5619
    },
    {
      name: "December", value: 1259.7252
    }
];


}
