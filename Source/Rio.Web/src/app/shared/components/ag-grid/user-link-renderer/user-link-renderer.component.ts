import { Component, OnInit } from '@angular/core';
import { AgRendererComponent } from 'ag-grid-angular';

@Component({
  selector: 'rio-user-link-renderer',
  templateUrl: './user-link-renderer.component.html',
  styleUrls: ['./user-link-renderer.component.scss']
})

export class UserLinkRendererComponent implements AgRendererComponent {
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