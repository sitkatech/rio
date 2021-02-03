import { Component, OnInit, Input, EventEmitter, Output, ViewChild, ElementRef } from '@angular/core';
import { WaterYearDto } from "src/app/shared/models/water-year-dto";

declare var $: any

@Component({
  selector: 'rio-water-year-select',
  templateUrl: './water-year-select.component.html',
  styleUrls: ['./water-year-select.component.scss']
})
export class WaterYearSelectComponent implements OnInit {
  @Input() waterYears: Array<WaterYearDto> = new Array<WaterYearDto>();
  @Input() disabled: Boolean;
  @Input() selectYearLabel: string = "Viewing year";
  @Input() textColor: string = "";
  @Input() defaultHover: boolean = true;

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

  @ViewChild('waterYearDropdown') waterYearDropdown: ElementRef<HTMLElement>
  

  constructor() { }

  ngOnInit() {
  }

  //If the component lives inside a parent component and needs to be triggered from clicking 
  //anywhere inside the parent component
  triggerDropdownToggle(event: Event) {
    event.stopPropagation();
    $(this.waterYearDropdown.nativeElement).dropdown('toggle');
    this.waterYearDropdown.nativeElement.blur();
  }

}
