import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { UserService } from 'src/app/services/user/user.service';
import { UserDto, ParcelChangeOwnerDto } from 'src/app/shared/models';

@Component({
  selector: 'rio-parcel-change-owner',
  templateUrl: './parcel-change-owner.component.html',
  styleUrls: ['./parcel-change-owner.component.scss']
})
export class ParcelChangeOwnerComponent implements OnInit {
  public users: UserDto[];
  public selectedUser: UserDto;
  // public selectListItems: { id: number, description: string }[] = []

  public config = {
    search: true,
    displayKey: "FullName",
    searchOnKey: "FullName"
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private parcelService: ParcelService,
    private authenticationService: AuthenticationService,
  ) { }

  ngOnInit() {
    this.userService.getUsers().subscribe(users => {
      this.users = users
    })

  }

  public getDisplayName(user: UserDto): string {
    return `${user.FirstName} ${user.LastName}`;
  }

  public selected() {
    console.log(this.selectedUser);
  }

}
