import { PostingTypeDto } from '../postingType/postingType-dto';
import { UserSimpleDto } from '../user/user-simple-dto';

export class PostingDto {
    PostingID: number;
    PostingType: PostingTypeDto;

    PostingDescription: string;
    Quantity: number;
    Price: number;

    PostingDate: Date;

    CreateUser: UserSimpleDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

