import { Component, OnInit } from '@angular/core';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'rio-glossary',
  templateUrl: './glossary.component.html',
  styleUrls: ['./glossary.component.scss']
})
export class GlossaryComponent implements OnInit {
  waterTypes: WaterTypeDto[];
  public purchasedCustomRichTextTypeID = CustomRichTextTypeEnum.PurchasedDescription;
  public soldCustomRichTextTypeID = CustomRichTextTypeEnum.SoldDescription;

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
