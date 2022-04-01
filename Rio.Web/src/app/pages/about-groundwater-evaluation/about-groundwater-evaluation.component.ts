import { Component, OnInit } from '@angular/core';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';

@Component({
  selector: 'rio-about-groundwater-evaluation',
  templateUrl: './about-groundwater-evaluation.component.html',
  styleUrls: ['./about-groundwater-evaluation.component.scss']
})
export class AboutGroundwaterEvaluationComponent implements OnInit {
  public richTextTypeID: number = CustomRichTextTypeEnum.AboutGET;
  constructor() { }

  ngOnInit() {
  }

}
