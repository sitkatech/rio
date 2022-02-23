import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { ParcelDto } from 'src/app/shared/generated/model/parcel-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';

@Component({
    selector: 'template-user-detail',
    templateUrl: './user-detail.component.html',
    styleUrls: ['./user-detail.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserDetailComponent implements OnInit, OnDestroy {
    
    private currentUser: UserDto;

    public user: UserDto;
    public parcels: Array<ParcelDto>;
    public accounts: Array<AccountSimpleDto>

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private userService: UserService,
        private parcelService: ParcelService,
        private authenticationService: AuthenticationService,
        private cdr: ChangeDetectorRef
    ) {
        // force route reload whenever params change;
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    }

    ngOnInit() {
        this.authenticationService.getCurrentUser().subscribe(currentUser => {
            this.currentUser = currentUser;
            const id = parseInt(this.route.snapshot.paramMap.get("id"));
            if (id) {
                forkJoin([this.userService.getUserFromUserID(id),
                    this.parcelService.getParcelsByUserID(id, new Date().getFullYear()),
                    this.userService.listAccountsByUserID(id)]
                ).subscribe(([user, parcels, accounts]) => {
                    this.user = user instanceof Array
                        ? null
                        : user as UserDto;
                    this.parcels = parcels;
                    this.accounts = accounts;
                    this.cdr.detectChanges();
                });
            }
        });
    }

    ngOnDestroy() {
        
        
        this.cdr.detach();
    }

    public currentUserIsAdmin(): boolean {
        return this.authenticationService.isUserAnAdministrator(this.currentUser);
    }

    public currentUserIsAdminOrDemoUser(): boolean {
        return this.authenticationService.isUserADemoUserOrAdministrator(this.currentUser);
    }

    public userIsAdministrator(): boolean{
        return this.authenticationService.isUserAnAdministrator(this.user);
    }

    public getSelectedParcelIDs(): Array<number> {
        return this.parcels !== undefined ? this.parcels.map(p => p.ParcelID) : [];
    }

    public canViewLandOwnerDashboard(): boolean {
        return this.currentUserIsAdmin();
    }

    public doOfferEditAccounts(): boolean{
        return this.currentUserIsAdmin() && !this.userIsAdministrator();
    }
}
