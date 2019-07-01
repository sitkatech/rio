import { Component, OnInit } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { UserService } from 'src/app/services/user/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';

@Component({
    selector: 'template-user-detail',
    templateUrl: './user-detail.component.html',
    styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent implements OnInit {

    public user: UserDto;
    public parcels: Array<ParcelDto>;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private userService: UserService,
        private parcelService: ParcelService,
        private authenticationService: AuthenticationService
    ) {
    }

    ngOnInit() {
        const id = parseInt(this.route.snapshot.paramMap.get("id"));
        if (id) {
            forkJoin(
                this.userService.getUserFromUserID(id),
                this.parcelService.getParcelsByUserID(id)
            ).subscribe(([user, parcels]) => {
                this.user = user instanceof Array
                    ? null
                    : user as UserDto;
                this.parcels = parcels;
            });
        }
    }

    public currentUserIsAdmin(): boolean {
        return this.authenticationService.canAdmin();
    }

    public getSelectedParcelIDs(): Array<number> {
        return this.parcels.map(p => p.ParcelID);
    }
}
