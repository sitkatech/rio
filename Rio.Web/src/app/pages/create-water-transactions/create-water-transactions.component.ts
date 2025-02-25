import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'rio-create-water-transactions',
  templateUrl: './create-water-transactions.component.html',
  styleUrls: ['./create-water-transactions.component.scss']
})
export class CreateWaterTransactionsComponent implements OnInit, OnDestroy {
  
  private currentUser: UserDto;
  public richTextTypeID: number = CustomRichTextTypeEnum.CreateWaterTransactions;

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

  public includeWaterSupply():boolean{
    return environment.includeWaterSupply;
  }
}
