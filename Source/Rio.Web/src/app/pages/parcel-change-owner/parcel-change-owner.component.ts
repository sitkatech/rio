import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { UserService } from 'src/app/services/user/user.service';
import { UserDto } from 'src/app/shared/models';
import { forkJoin } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { IMyDpOptions } from 'mydatepicker';
import { ParcelChangeOwnerDto } from 'src/app/shared/models/parcel/parcel-change-owner-dto';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';

@Component({
  selector: 'rio-parcel-change-owner',
  templateUrl: './parcel-change-owner.component.html',
  styleUrls: ['./parcel-change-owner.component.scss']
})
export class ParcelChangeOwnerComponent implements OnInit {
  public users: UserDto[];
  public selectedUser: UserDto;
  public parcelID: number;
  public ownerHasAccount: boolean;
  public effectiveYear: number;
  public saleDate: any;
  public parcel: ParcelDto;
  public ownerNameUntracked: string;
  public note: string;

  public isLoadingSubmit: boolean = false;


  public myDatePickerOptions: IMyDpOptions = {
    // other options...
    dateFormat: 'dd.mm.yyyy',
  };

  public userDropdownConfig = {
    search: true,
    height: '320px',
    placeholder: "Select an owner from the list of users",
    displayKey: "FullName",
    searchOnKey: "FullName",
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private parcelService: ParcelService,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.parcelID = parseInt(this.route.snapshot.paramMap.get("id"));
    this.ownerHasAccount = true;
    forkJoin(
      this.userService.getUsers(),
      this.parcelService.getParcelByParcelID(this.parcelID)
    ).subscribe(([users, parcel]) => {
      this.users = users;
      this.parcel = parcel;
    });

  }

  public getDisplayName(user: UserDto): string {
    return `${user.FirstName} ${user.LastName}`;
  }
  public selected() {
    console.log(this.selectedUser);
  }

  public onSubmit(form: HTMLFormElement){
    var associativeArray = {
      ParcelID: this.parcelID,
      UserID: this.selectedUser ? this.selectedUser.UserID : undefined,
      OwnerName: this.ownerNameUntracked,
      SaleDate: this.saleDate.jsdate,
      EffectiveYear: this.effectiveYear,
      Note: this.note

    }
    var parcelChangeOwnerDto = new ParcelChangeOwnerDto(associativeArray);
    this.parcelService.changeParcelOwner(this.parcelID, parcelChangeOwnerDto).subscribe(anything=>{
      this.isLoadingSubmit = false;
      form.reset();
      this.router.navigateByUrl(`/parcels/${this.parcelID}`).then(x=> {
        this.alertService.pushAlert(new Alert(`The ownership record for ${this.parcel.ParcelNumber} was successfully updated.`, AlertContext.Success ));
      })
    }, error=>{
      this.isLoadingSubmit = false;
      this.cdr.detectChanges();
    });
  }

}
