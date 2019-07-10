import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';

@Component({
  selector: 'rio-parcels-home',
  templateUrl: './parcels-home.component.html',
  styleUrls: ['./parcels-home.component.scss']
})
export class ParcelsHomeComponent implements OnInit {

  parcels: Array<ParcelDto>;

  constructor(private cdr: ChangeDetectorRef, private parcelService: ParcelService) { }

  ngOnInit() {
    this.parcelService.getParcelsWithLandOwners().subscribe(result => {
      this.parcels = result;
      this.cdr.detectChanges();
    });
  }


  public getSelectedParcelIDs(): Array<number> {
    return this.parcels !== undefined ? this.parcels.map(p => p.ParcelID) : [];
  }
}
