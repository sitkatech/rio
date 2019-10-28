import { OfferDto } from './offer/offer-dto';
import { WaterTransferDto } from './water-transfer-dto';

export class MarketMetricsDto {
    MostRecentOfferToBuyPrice: number;
    MostRecentOfferToBuyQuantity: number;
    MostRecentOfferToSellPrice: number;
    MostRecentOfferToSellQuantity: number;
    MostRecentWaterTransfer: WaterTransferDto;
    TotalBuyVolume: number;
    TotalSellVolume: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

