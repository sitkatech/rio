import { TradeStatusDto } from './trade-status-dto';
import { PostingDto } from '../posting/posting-dto';
import { AccountDto } from '../account/account-dto';

export class TradeDto {
    TradeID: number;
    TradeNumber: string;
    CreateAccount: AccountDto;
    TradeStatus: TradeStatusDto;
    Posting: PostingDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
