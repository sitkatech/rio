import { UserSimpleDto } from '../user/user-simple-dto';
import { TradeStatusDto } from './trade-status-dto';
import { OfferStatusDto } from './offer-status-dto';

export class TradeWithMostRecentOfferDto {
    TradeID: number;
    TradeNumber: string;
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
    IsRegisteredByBuyer: boolean;
    IsRegisteredBySeller: boolean;
    WaterTransferID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

