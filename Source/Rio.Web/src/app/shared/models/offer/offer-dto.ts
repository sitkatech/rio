import { OfferStatusDto } from './offer-status-dto';
import { AccountSimpleDto } from '../account/account-simple-dto';

export class OfferDto {
    OfferID: number;
    OfferDate: Date;
    Quantity: number;
    Price: number;
    OfferNotes: string;
    CreateAccount: AccountSimpleDto;
    OfferStatus: OfferStatusDto;
    TradeID: number;
    WaterTransferID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

