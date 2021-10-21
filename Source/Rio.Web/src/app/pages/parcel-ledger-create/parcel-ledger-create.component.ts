import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { WaterTypeDto } from 'src/app/shared/models/water-type-dto';
import { WaterTypeService } from 'src/app/services/water-type.service';

@Component({
  selector: 'rio-parcel-ledger-create',
  templateUrl: './parcel-ledger-create.component.html',
  styleUrls: ['./parcel-ledger-create.component.scss']
})
export class ParcelLedgerCreateComponent implements OnInit {

  private watchUserChangeSubscription: any;
  public parcel: ParcelDto;
  public waterTypes: WaterTypeDto[];
  public adjustmentValue: string;
  
  constructor(
    private authenticationService: AuthenticationService,
    private route: ActivatedRoute,
    private parcelService: ParcelService,
    private waterTypeService: WaterTypeService,
  ) { }

  ngOnInit(): void {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(() => {
      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      forkJoin(
        this.parcelService.getParcelByParcelID(id),
        this.waterTypeService.getWaterTypes()
      ).subscribe(
        ([parcel, waterTypes]) => {
          this.parcel = parcel instanceof Array
            ? null
            : parcel as ParcelDto;
          this.parcel = parcel;
          this.waterTypes = waterTypes;
        }
      );
    });

    this.adjustmentValue = "supply";
  }
}

