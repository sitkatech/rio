import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'rio-water-year-select',
  templateUrl: './water-year-select.component.html',
  styleUrls: ['./water-year-select.component.scss']
})
export class WaterYearSelectComponent implements OnInit {
  @Input() years: Array<number> = new Array<number>();
  @Input() disabled: Boolean;
  @Input() selectYearLabel: string = "Viewing year";

  @Input()
  get selectedYear(): number {
    return this.selectedYearValue
  }

  set selectedYear(val: number) {
    this.selectedYearValue = val;
    this.selectedYearChange.emit(this.selectedYearValue);
  }

  selectedYearValue: number;

  @Output() selectedYearChange: EventEmitter<number> = new EventEmitter<number>();
  

  constructor() { }

  ngOnInit() {
  }

}
