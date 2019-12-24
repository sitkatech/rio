import { PostingTypeDto } from './posting-type-dto';
import { PostingStatusDto } from './posting-status-dto';
import { AccountDto } from '../account/account-dto';

export class PostingDto {
    PostingID: number;
    PostingType: PostingTypeDto;
    PostingStatus: PostingStatusDto;

    PostingDescription: string;
    Quantity: number;
    AvailableQuantity: number;
    Price: number;

    PostingDate: Date;

    CreateAccount: AccountDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

