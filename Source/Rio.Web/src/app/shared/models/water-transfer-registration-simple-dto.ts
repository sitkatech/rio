import { UserSimpleDto } from './user/user-simple-dto';

export class WaterTransferRegistrationSimpleDto {
    WaterTransferTypeID: number;
    User: UserSimpleDto;
    WaterTransferRegistrationStatusID: number;
    StatusDate: Date;
    IsRegistered: boolean;
    IsCanceled: boolean;
    IsPending: boolean;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

