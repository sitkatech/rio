import { Component, OnInit } from '@angular/core';
import { AgRendererComponent } from 'ag-grid-angular';

@Component({
  selector: 'rio-land-owner-link-renderer',
  templateUrl: './land-owner-link-renderer.component.html',
  styleUrls: ['./land-owner-link-renderer.component.scss']
})

export class LandOwnerLinkRendererComponent implements AgRendererComponent {
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