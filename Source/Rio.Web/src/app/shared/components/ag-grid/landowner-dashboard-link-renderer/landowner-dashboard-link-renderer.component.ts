import { Component, OnInit } from '@angular/core';
import { AgRendererComponent } from 'ag-grid-angular';

@Component({
  selector: 'rio-landowner-dashboard-link-renderer',
  templateUrl: './landowner-dashboard-link-renderer.component.html',
  styleUrls: ['./landowner-dashboard-link-renderer.component.scss']
})
export class LandownerDashboardLinkRendererComponent implements AgRendererComponent {
  params: any;    

  agInit(params: any): void {
    if(params.value === null)
    {
      params = { value: "" }
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