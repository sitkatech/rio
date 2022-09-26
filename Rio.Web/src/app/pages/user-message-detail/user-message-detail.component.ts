import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserMessageService } from 'src/app/services/user-message.service';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { UserMessageDto } from 'src/app/shared/generated/model/user-message-dto';

@Component({
  selector: 'fresca-user-message-detail',
  templateUrl: './user-message-detail.component.html',
  styleUrls: ['./user-message-detail.component.scss']
})
export class UserMessageDetailComponent implements OnInit {
  private currentUser: UserDto;
  public userMessage: UserMessageDto;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userMessageService: UserMessageService,
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
        this.userMessageService.getUserMessageFromUserMessageID(id).subscribe(userMessage => {
          this.userMessage = userMessage;
          this.cdr.detectChanges();
        });
      }
    });
  }

  ngOnDestroy() {
    this.cdr.detach();
  }
}
