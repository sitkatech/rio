import { Component } from '@angular/core';
import { AgFilterComponent } from 'ag-grid-angular';
import { IDoesFilterPassParams, RowNode } from 'ag-grid-community';

@Component({
  selector: 'rio-dropdown-select-component',
  templateUrl: './dropdown-select-filter.component.html',
  styleUrls: ['./dropdown-select-filter.component.scss']
})
export class DropdownSelectFilterComponent implements AgFilterComponent {

  params;
  field: string;
  dropdownValues = [];

  state = {
    selectAll: true,
    deselectAll: false
  };

  agInit(params): void {
    this.params = params;
    this.field = params.colDef.filterParams.field;

    this.params.api.forEachNode((rowNode, i) => {
      let value = this.getDisplayNameForNode(rowNode);
      if (!this.dropdownValues.includes(value)) {
        this.dropdownValues.push(value);
      }
    });

    this.dropdownValues.forEach(element => {
      this.state[element] = true;
    });
  }

  isFilterActive(): boolean {
    return !this.state.selectAll;
  }

  doesFilterPass(filterParams: IDoesFilterPassParams): boolean {
    let displayName = this.getDisplayNameForNode(filterParams.node);
    
    if (this.state[displayName] == null) {
      return false;
    }
    
    return this.state[displayName] ? true : false;
  }
  
  private getDisplayNameForNode(rowNode: RowNode) {
    if (this.params.colDef.valueGetter) {
      return this.params.colDef.valueGetter(rowNode);
    }

    return this.getPropertyValue(rowNode.data, this.field, '');
   }

   
  private getPropertyValue(object, path, defaultValue) {
    return path
      .split('.')
      .reduce((o, p) => o ? o[p] : defaultValue, object);
  } 


  getModel() {
    return {filtersActive: this.state};
  }

  setModel(model: any) {
    if (model === null) {
      this.onSelectAll();
    } else {
      this.state = model.filtersActive;
    }
  }

  getDropdownValues()
  {
    return this.dropdownValues.sort();
  }

  updateFilter() {
    this.state.selectAll = true;
    this.state.deselectAll = true;

    for (let element of this.dropdownValues) {
      if (this.state[element]) {
        this.state.deselectAll = false;
      } else {
        this.state.selectAll = false;
      }

      if (!this.state.selectAll && !this.state.deselectAll) {
        break;
      }
    };

    this.params.filterChangedCallback();
  }
  
  onSelectAll() {
    this.state.selectAll = true;
    this.state.deselectAll = false;
    
    this.updateFilterSelection();
  }
  
  onDeselectAll() {
    this.state.selectAll = false;
    this.state.deselectAll = true;

    this.updateFilterSelection();
  }

  private updateFilterSelection() {
    this.dropdownValues.forEach(element => {
      this.state[element] = this.state.selectAll;
    });

    this.params.filterChangedCallback();
  }
}
