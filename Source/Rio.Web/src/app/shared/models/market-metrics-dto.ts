import { OfferDto } from './offer/offer-dto';
import { WaterTransferDto } from './water-transfer-dto';

export class MarketMetricsDto {
    MostRecentOfferToBuy: OfferDto;
    MostRecentOfferToSell: OfferDto;
    MostRecentWaterTransfer: WaterTransferDto;
    TotalBuyVolume: number;
    TotalSellVolume: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

