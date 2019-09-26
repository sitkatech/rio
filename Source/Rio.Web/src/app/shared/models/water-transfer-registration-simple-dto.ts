import { UserSimpleDto } from './user/user-simple-dto';

export class WaterTransferRegistrationSimpleDto {
    RegisteredDate: Date;
    WaterTransferTypeID: number;
    User: UserSimpleDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

