import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { UserDto } from 'src/app/shared/generated/model/user-dto';

@Component({
  selector: 'rio-create-water-transactions',
  templateUrl: './create-water-transactions.component.html',
  styleUrls: ['./create-water-transactions.component.scss']
})
export class CreateWaterTransactionsComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  public richTextTypeID: number = CustomRichTextType.CreateWaterTransactions;

  constructor(
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService) { }

  ngOnInit(): void {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }
}
