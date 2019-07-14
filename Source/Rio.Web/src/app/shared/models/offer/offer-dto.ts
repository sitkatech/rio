import { OfferStatusDto } from './offer-status-dto';
import { UserSimpleDto } from '../user/user-simple-dto';

export class OfferDto {
    OfferID: number;
    OfferDate: Date;
    Quantity: number;
    Price: number;
    OfferNotes: string;
    CreateUser: UserSimpleDto;
    OfferStatus: OfferStatusDto;
    TradeID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

