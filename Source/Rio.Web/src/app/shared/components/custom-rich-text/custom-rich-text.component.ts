import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CustomRichTextService } from '../../services/custom-rich-text.service';

@Component({
  selector: 'rio-custom-rich-text',
  templateUrl: './custom-rich-text.component.html',
  styleUrls: ['./custom-rich-text.component.scss']
})
export class CustomRichTextComponent implements OnInit, OnChanges {
  @Input() customRichTextTypeID: number;
  public customRichTextContent: string;
  public isLoading: boolean = true;

  constructor(private customRichTextService: CustomRichTextService) { }
  
  ngOnInit() {
    console.log(this.customRichTextTypeID);
    this.customRichTextService.getCustomRichText(this.customRichTextTypeID).subscribe(x=>{
      this.customRichTextContent = x.CustomRichTextContent;
      this.isLoading = false;
    })
  }

  ngOnChanges(changes: SimpleChanges): void {
  }
}
