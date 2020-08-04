import { Component, OnInit } from '@angular/core';
import { AgRendererComponent } from 'ag-grid-angular';

@Component({
  selector: 'rio-link-renderer',
  templateUrl: './link-renderer.component.html',
  styleUrls: ['./link-renderer.component.scss']
})

export class LinkRendererComponent implements AgRendererComponent {
  params: any;    

  agInit(params: any): void {
    this.params = params;
  }

  refresh(params: any): boolean {
      return false;
  }    
}