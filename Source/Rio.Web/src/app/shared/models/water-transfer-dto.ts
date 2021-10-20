import { WaterTransferRegistrationSimpleDto } from './water-transfer-registration-simple-dto';

export class WaterTransferDto {
    WaterTransferID: number;
    OfferID: number;
    TransferDate: Date;
    TransferYear: number;
    AcreFeetTransferred: number;
    UnitPrice: number;
    BuyerRegistration: WaterTransferRegistrationSimpleDto;
    SellerRegistration: WaterTransferRegistrationSimpleDto;
    Notes: string;
    TradeNumber: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

