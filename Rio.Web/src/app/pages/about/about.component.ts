import { Component, OnInit } from '@angular/core';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';

@Component({
  selector: 'rio-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.scss']
})
export class AboutComponent implements OnInit {
  public richTextTypeID : number = CustomRichTextTypeEnum.PlatformOverview;
  constructor() { }

  ngOnInit() {
  }

}
