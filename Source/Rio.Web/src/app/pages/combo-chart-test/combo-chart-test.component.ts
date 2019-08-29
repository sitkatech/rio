import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'rio-combo-chart-test',
  templateUrl: './combo-chart-test.component.html',
  styleUrls: ['./combo-chart-test.component.scss']
})
export class ComboChartTestComponent {
  view = [500,400];
  showXAxis = true;
  showYAxis = true;
  gradient = false;
  showLegend = true;
  legendTitle = 'Legend';
  legendPosition = 'right';
  showXAxisLabel = true;
  xAxisLabel = 'Country';
  showYAxisLabel = true;
  yAxisLabel = 'GDP Per Capita';
  showGridLines = true;
  innerPadding = '10%';
  animations: boolean = true;
  barChart: any[] = barChart;
  lineChartSeries: any[] = lineChartSeries;
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

  showRightYAxisLabel: boolean = true;
  yAxisLabelRight: string = 'Utilization';
}

export let lineChartSeries = [
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

export let barChart: any = [
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
