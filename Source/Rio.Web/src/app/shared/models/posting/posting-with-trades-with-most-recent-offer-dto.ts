import { UserSimpleDto } from '../user/user-simple-dto';
import { PostingTypeDto } from './posting-type-dto';
import { PostingStatusDto } from './posting-status-dto';
import { TradeWithMostRecentOfferDto } from '../offer/trade-with-most-recent-offer-dto';

export class PostingWithTradesWithMostRecentOfferDto {
    PostingID: number;
    PostingType: PostingTypeDto;
    PostingStatus: PostingStatusDto;

    PostingDescription: string;
    Quantity: number;
    AvailableQuantity: number;
    Price: number;

    PostingDate: Date;

    CreateUser: UserSimpleDto;

    Trades: Array<TradeWithMostRecentOfferDto>;
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

