import { Component, OnInit } from '@angular/core';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';

@Component({
  selector: 'rio-training-videos',
  templateUrl: './training-videos.component.html',
  styleUrls: ['./training-videos.component.scss']
})
export class TrainingVideosComponent implements OnInit {
  public richTextTypeID: number = CustomRichTextTypeEnum.TrainingVideos;
  
  constructor() { }

  ngOnInit() {
  }

}
