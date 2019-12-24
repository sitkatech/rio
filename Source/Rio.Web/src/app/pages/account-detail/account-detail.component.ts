import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { AccountSimpleDto } from 'src/app/shared/models/account/account-simple-dto';
import { ActivatedRoute, Router } from '@angular/router';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AccountService } from 'src/app/services/account/account.service';
import { AccountDto } from 'src/app/shared/models/account/account-dto';

@Component({
  selector: 'rio-account-detail',
  templateUrl: './account-detail.component.html',
  styleUrls: ['./account-detail.component.scss']
})
export class AccountDetailComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public account: AccountDto;
  public parcels: Array<ParcelDto>;
  public accounts: Array<AccountSimpleDto>

  constructor(
      private route: ActivatedRoute,
      private router: Router,
      private accountService: AccountService,
      private parcelService: ParcelService,
      private authenticationService: AuthenticationService,
      private cdr: ChangeDetectorRef
  ) {
      // force route reload whenever params change;
      this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
      this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
          this.currentUser = currentUser;
          const id = parseInt(this.route.snapshot.paramMap.get("id"));
          if (id) {
              this.accountService.getAccountByID(id).subscribe(account =>{
                this.account = account;
              });
          }
      });
  }

  ngOnDestroy() {
      this.watchUserChangeSubscription.unsubscribe();
      this.authenticationService.dispose();
      this.cdr.detach();
  }
  
  public currentUserIsAdmin(): boolean {
      return this.authenticationService.isUserAnAdministrator(this.currentUser);
  }
}
