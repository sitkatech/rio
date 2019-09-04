import { Component, OnInit } from '@angular/core';
import { AgRendererComponent } from 'ag-grid-angular';

@Component({
  selector: 'rio-parcel-number-link-renderer',
  templateUrl: './parcel-number-link-renderer.component.html',
  styleUrls: ['./parcel-number-link-renderer.component.scss']
})
export class ParcelNumberLinkRendererComponent implements AgRendererComponent {    
  params: any;    

  agInit(params: any): void {
    if(params.value === null)
    {
      params = { value: "", inRouterLink: ""}
    }
    else
    {
      this.params = params;
    }
  }

  refresh(params: any): boolean {
      return false;
  }    
}