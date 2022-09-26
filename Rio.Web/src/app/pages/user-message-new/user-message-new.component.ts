import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserMessageService } from 'src/app/services/user-message.service';
import { UserService } from 'src/app/services/user/user.service';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { UserMessageSimpleDto } from 'src/app/shared/generated/model/user-message-simple-dto';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'fresca-user-message-new',
  templateUrl: './user-message-new.component.html',
  styleUrls: ['./user-message-new.component.scss']
})
export class UserMessageNewComponent implements OnInit {
  private currentUser: UserDto;

  public model: UserMessageSimpleDto;
  public isLoadingSubmit: boolean = false;
  public users: UserDto[];

  constructor(private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private userMessageService: UserMessageService,
    private authenticationService: AuthenticationService,
    private alertService: AlertService) { }

  ngOnInit(): void {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;

      this.userService.getUsers().subscribe(result => {
        this.users = result;
        this.cdr.detectChanges();
      });

      this.model = new UserMessageSimpleDto();
    });
  }

  onSubmit(newMessageForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;

    this.userMessageService.createNewUserMessage(this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        this.router.navigateByUrl("/user-messages").then(x => {
          this.alertService.pushAlert(new Alert("The message was successfully sent.", AlertContext.Success));
        });
      }
        ,
        error => {
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }

  ngOnDestroy() {
    this.cdr.detach();
  }
}
