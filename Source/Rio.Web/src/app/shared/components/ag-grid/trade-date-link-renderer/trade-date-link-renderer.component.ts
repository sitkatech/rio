import { Component, OnInit } from '@angular/core';
import { AgRendererComponent } from 'ag-grid-angular';

@Component({
  selector: 'rio-trade-date-link-renderer',
  templateUrl: './trade-date-link-renderer.component.html',
  styleUrls: ['./trade-date-link-renderer.component.scss']
})
export class TradeDateLinkRendererComponent implements AgRendererComponent {    
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