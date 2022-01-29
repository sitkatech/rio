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
  
  private currentUser: UserDto;
  public richTextTypeID: number = CustomRichTextType.CreateWaterTransactions;

  constructor(
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService) { }

  ngOnInit(): void {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
    });
  }

  ngOnDestroy() {
    
    
    this.cdr.detach();
  }
}
