import { Component, OnInit } from '@angular/core';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';

@Component({
  selector: 'rio-general-faq',
  templateUrl: './general-faq.component.html',
  styleUrls: ['./general-faq.component.scss']
})
export class GeneralFaqComponent implements OnInit {
  public richTextTypeID: number = CustomRichTextTypeEnum.FrequentlyAskedQuestions;
  
  constructor() { }

  ngOnInit() {
  }

}
