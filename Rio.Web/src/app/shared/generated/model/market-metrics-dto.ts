/**
 * Rio.API
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * OpenAPI spec version: 1.0
 * 
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */
import { WaterTransferDto } from '././water-transfer-dto';

export class MarketMetricsDto { 
    MostRecentOfferToBuyQuantity?: number;
    MostRecentOfferToBuyPrice?: number;
    MostRecentOfferToSellQuantity?: number;
    MostRecentOfferToSellPrice?: number;
    MostRecentWaterTransfer?: WaterTransferDto;
    TotalBuyVolume?: number;
    TotalSellVolume?: number;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}