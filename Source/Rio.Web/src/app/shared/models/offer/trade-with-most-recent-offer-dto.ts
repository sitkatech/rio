import { UserSimpleDto } from '../user/user-simple-dto';
import { TradeStatusDto } from './trade-status-dto';
import { OfferStatusDto } from './offer-status-dto';

export class TradeWithMostRecentOfferDto {
    TradeID: number;
    PostingID: number;
    CreateUser: UserSimpleDto;
    TradeStatus: TradeStatusDto;
    OfferID: number;
    Quantity: number;
    Price: number;
    OfferDate: Date;
    OfferStatus: OfferStatusDto

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

