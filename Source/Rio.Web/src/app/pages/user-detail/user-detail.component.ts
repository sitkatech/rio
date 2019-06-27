import { Component, OnInit } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { UserService } from 'src/app/services/user/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'template-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent implements OnInit {

  public user: UserDto;

  constructor(
      private route: ActivatedRoute,
      private router: Router,
      private userService: UserService,
      private authenticationService: AuthenticationService
  ) {
  }

  ngOnInit() {
      this.getUser();
  }

  private getUser(): void {
      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      if (id) {
          this.userService.getUserFromUserID(id)
              .subscribe(resource => {
                  this.user = resource instanceof Array
                      ? null
                      : resource as UserDto;
              });
      }
  }

  public currentUserIsAdmin(): boolean {
      return this.authenticationService.canAdmin();
  }
}
