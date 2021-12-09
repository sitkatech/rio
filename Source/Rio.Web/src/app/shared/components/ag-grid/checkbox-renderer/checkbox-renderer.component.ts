import { Component } from '@angular/core';
import { AgRendererComponent } from 'ag-grid-angular';


@Component({
  selector: 'rio-checkbox-renderer',
  templateUrl: './checkbox-renderer.component.html',
  styleUrls: ['./checkbox-renderer.component.scss']
})
export class CheckboxRendererComponent implements AgRendererComponent {
  params: any;    

  agInit(params: any): void {
    this.params = params;
  }

  public callOnChangeFunction() {
    if (!this.params.value || !this.params.value.onChangeFunctionParams) {
      this.params._self[this.params.onChangeFunction].apply(this.params._self);
      return; 
    }

    this.params._self[this.params.onChangeFunction].apply(this.params._self, this.params.value.onChangeFunctionParams);
  }

  refresh(params: any): boolean {
      return false;
  } 
}
