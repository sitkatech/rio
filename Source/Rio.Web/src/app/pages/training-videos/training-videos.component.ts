import { Component, OnInit } from '@angular/core';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-training-videos',
  templateUrl: './training-videos.component.html',
  styleUrls: ['./training-videos.component.scss']
})
export class TrainingVideosComponent implements OnInit {
  public richTextTypeID: number = CustomRichTextType.TrainingVideos;
  
  constructor() { }

  ngOnInit() {
  }

}
