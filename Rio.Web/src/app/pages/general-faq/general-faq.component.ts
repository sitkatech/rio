import { Component, OnInit } from '@angular/core';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-general-faq',
  templateUrl: './general-faq.component.html',
  styleUrls: ['./general-faq.component.scss']
})
export class GeneralFaqComponent implements OnInit {
  public richTextTypeID: number = CustomRichTextType.FrequentlyAskedQuestions;
  
  constructor() { }

  ngOnInit() {
  }

}
