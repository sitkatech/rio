import { UserSimpleDto } from './user/user-simple-dto';

export class WaterConfirmationDto {
    ConfirmationDate: Date;
    ConfirmationType: string;
    ConfirmedBy: UserSimpleDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

