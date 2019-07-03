import { PostingTypeDto } from '../postingType/postingType-dto';
import { UserDto } from '../user/user-dto';

export class PostingDto {
    PostingID: number;
    PostingType: PostingTypeDto;

    PostingDescription: string;
    Quantity: number;
    Price: number;

    PostingDate: Date;

    CreateUser: UserDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

