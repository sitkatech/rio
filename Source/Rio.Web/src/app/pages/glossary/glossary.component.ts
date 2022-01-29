import { Component, OnInit } from '@angular/core';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'rio-glossary',
  templateUrl: './glossary.component.html',
  styleUrls: ['./glossary.component.scss']
})
export class GlossaryComponent implements OnInit {
  waterTypes: WaterTypeDto[];
  public precipCustomRichTextTypeID = CustomRichTextType.PrecipitationDescription;
  public purchasedCustomRichTextTypeID = CustomRichTextType.PurchasedDescription;
  public soldCustomRichTextTypeID = CustomRichTextType.SoldDescription;

  constructor(
    private waterTypeService: WaterTypeService
  ) { }

  ngOnInit() {
    this.waterTypeService.getWaterTypes().subscribe(x=>{
      this.waterTypes = x;
    })
  }

  public allowTrading(): boolean {
    return environment.allowTrading;
  }
}
