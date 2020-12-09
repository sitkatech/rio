import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { WaterYearDto } from 'src/app/shared/models/openet-sync-history-dto';

@Component({
  selector: 'rio-water-year-select',
  templateUrl: './water-year-select.component.html',
  styleUrls: ['./water-year-select.component.scss']
})
export class WaterYearSelectComponent implements OnInit {
  @Input() waterYears: Array<WaterYearDto> = new Array<WaterYearDto>();
  @Input() disabled: Boolean;
  @Input() selectYearLabel: string = "Viewing year";

  @Input()
  get selectedYear(): WaterYearDto {
    return this.selectedYearValue
  }

  set selectedYear(val: WaterYearDto) {
    this.selectedYearValue = val;
    this.selectedYearChange.emit(this.selectedYearValue);
  }

  selectedYearValue: WaterYearDto;

  @Output() selectedYearChange: EventEmitter<WaterYearDto> = new EventEmitter<WaterYearDto>();
  

  constructor() { }

  ngOnInit() {
  }

}
