import { PostingTypeDto } from './posting-type-dto';
import { UserSimpleDto } from '../user/user-simple-dto';
import { PostingStatusDto } from './posting-status-dto';

export class PostingDto {
    PostingID: number;
    PostingType: PostingTypeDto;
    PostingStatus: PostingStatusDto;

    PostingDescription: string;
    Quantity: number;
    AvailableQuantity: number;
    Price: number;

    PostingDate: Date;

    CreateUser: UserSimpleDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

