import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';

@Component({
    selector: 'template-parcel-detail',
    templateUrl: './parcel-detail.component.html',
    styleUrls: ['./parcel-detail.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ParcelDetailComponent implements OnInit {

    public parcel: ParcelDto;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private parcelService: ParcelService,
        private authenticationService: AuthenticationService,
        private cdr: ChangeDetectorRef
    ) {
    }

    ngOnInit() {
        const id = parseInt(this.route.snapshot.paramMap.get("id"));
        if (id) {
            forkJoin(
                this.parcelService.getParcelByParcelID(id)
            ).subscribe(([parcel]) => {
                this.parcel = parcel instanceof Array
                    ? null
                    : parcel as ParcelDto;
                this.parcel = parcel;
                this.cdr.detectChanges();
            });
        }
    }

    public getSelectedParcelIDs(): Array<number> {
        return this.parcel !== undefined ? [this.parcel.ParcelID] : [];
    }
}
