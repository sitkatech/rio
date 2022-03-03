import { Component, OnInit } from '@angular/core';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-about-groundwater-evaluation',
  templateUrl: './about-groundwater-evaluation.component.html',
  styleUrls: ['./about-groundwater-evaluation.component.scss']
})
export class AboutGroundwaterEvaluationComponent implements OnInit {
  public richTextTypeID: number = CustomRichTextType.AboutGET;
  constructor() { }

  ngOnInit() {
  }

}
