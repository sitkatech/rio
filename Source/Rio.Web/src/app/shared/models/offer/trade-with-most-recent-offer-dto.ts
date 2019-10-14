import { UserSimpleDto } from '../user/user-simple-dto';
import { TradeStatusDto } from './trade-status-dto';
import { OfferStatusDto } from './offer-status-dto';
import { WaterTransferRegistrationSimpleDto } from '../water-transfer-registration-simple-dto';

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
    BuyerRegistration: WaterTransferRegistrationSimpleDto;
    SellerRegistration: WaterTransferRegistrationSimpleDto;
    WaterTransferID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

