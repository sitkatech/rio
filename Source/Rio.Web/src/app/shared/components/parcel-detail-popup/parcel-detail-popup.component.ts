import { Component, OnInit, DoCheck, ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'rio-parcel-detail-popup',
  templateUrl: './parcel-detail-popup.component.html',
  styleUrls: ['./parcel-detail-popup.component.scss']
})
export class ParcelDetailPopupComponent implements OnInit {
  constructor(private cdr: ChangeDetectorRef) { }

  ngOnInit() {
  }

  public detectChanges() : void{
    this.cdr.detectChanges();
  } 
}
