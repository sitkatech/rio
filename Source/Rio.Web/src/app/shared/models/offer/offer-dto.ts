import { OfferStatusDto } from './offer-status-dto';
import { AccountSimpleDto } from '../account/account-simple-dto';
import { TradeDto } from './trade-dto';

export class OfferDto {
    OfferID: number;
    OfferDate: Date;
    Quantity: number;
    Price: number;
    OfferNotes: string;
    CreateAccount: AccountSimpleDto;
    OfferStatus: OfferStatusDto;
    Trade: TradeDto;
    WaterTransferID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

