import { UserSimpleDto } from '../user/user-simple-dto';
import { TradeStatusDto } from './trade-status-dto';
import { OfferStatusDto } from './offer-status-dto';
import { WaterTransferRegistrationSimpleDto } from '../water-transfer-registration-simple-dto';
import { AccountSimpleDto } from '../account/account-simple-dto';

export class TradeWithMostRecentOfferDto {
    TradeID: number;
    TradeNumber: string;
    TradeStatus: TradeStatusDto;
    TradePostingTypeID: number;
    OfferPostingTypeID: number;
    Quantity: number;
    Price: number;
    OfferDate: Date;
    OfferCreateAccount: AccountSimpleDto;
    OfferCreatedAccountUser?: UserSimpleDto;
    OfferStatus: OfferStatusDto;
    Buyer: AccountSimpleDto;
    Seller: AccountSimpleDto;
    BuyerRegistration: WaterTransferRegistrationSimpleDto;
    SellerRegistration: WaterTransferRegistrationSimpleDto;
    WaterTransferID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

