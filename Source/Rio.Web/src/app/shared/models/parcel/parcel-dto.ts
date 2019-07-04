import { UserSimpleDto } from '../user/user-simple-dto';

export class ParcelDto {
    ParcelID: number;
    ParcelNumber: string;
    ParcelAreaInAcres: number;
    LandOwner: UserSimpleDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}