import { Component, OnInit, Input, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { CustomRichTextService } from '../../services/custom-rich-text.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from '../../models';

@Component({
  selector: 'rio-custom-rich-text',
  templateUrl: './custom-rich-text.component.html',
  styleUrls: ['./custom-rich-text.component.scss']
})
export class CustomRichTextComponent implements OnInit, OnChanges {
  @Input() customRichTextTypeID: number;
  public customRichTextContent: string;
  public isLoading: boolean = true;
  public isEmptyContent: boolean = false;
  public watchUserChangeSubscription: any;
  currentUser: UserDto;

  constructor(private customRichTextService: CustomRichTextService, private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef ) { }
  
  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => { 
        this.currentUser = currentUser;
    });
    

    this.customRichTextService.getCustomRichText(this.customRichTextTypeID).subscribe(x=>{
      this.customRichTextContent = x.CustomRichTextContent;
      this.isEmptyContent = x.IsEmptyContent;
      this.isLoading = false;
    });
  }

  public showEditButton() : boolean{
    return this.authenticationService.isUserAnAdministrator(this.currentUser);
  }

  ngOnChanges(changes: SimpleChanges): void {
  }
}
