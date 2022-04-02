import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';

@Component({
  selector: 'rio-help',
  templateUrl: './help.component.html',
  styleUrls: ['./help.component.scss']
})
export class HelpComponent implements OnInit {
  public richTextTypeID: number = CustomRichTextTypeEnum.Contact;

  constructor() { }

  ngOnInit() {
  }
}
