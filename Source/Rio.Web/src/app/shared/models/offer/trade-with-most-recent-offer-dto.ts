import { UserSimpleDto } from '../user/user-simple-dto';
import { TradeStatusDto } from './trade-status-dto';
import { OfferStatusDto } from './offer-status-dto';
import { PostingDto } from '../posting/posting-dto';

export class TradeWithMostRecentOfferDto {
    TradeID: number;
    TradeStatus: TradeStatusDto;
    TradePostingTypeID: number;
    OfferPostingTypeID: number;
    Quantity: number;
    Price: number;
    OfferDate: Date;
    OfferCreateUser: UserSimpleDto;
    OfferStatus: OfferStatusDto;
    Buyer: UserSimpleDto;
    Seller: UserSimpleDto;
    IsConfirmedByBuyer: boolean;
    IsConfirmedBySeller: boolean;
    WaterTransferID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

