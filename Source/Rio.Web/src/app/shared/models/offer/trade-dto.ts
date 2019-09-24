import { UserSimpleDto } from '../user/user-simple-dto';
import { TradeStatusDto } from './trade-status-dto';
import { PostingDto } from '../posting/posting-dto';

export class TradeDto {
    TradeID: number;
    TradeNumber: string;
    CreateUser: UserSimpleDto;
    TradeStatus: TradeStatusDto;
    Posting: PostingDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
