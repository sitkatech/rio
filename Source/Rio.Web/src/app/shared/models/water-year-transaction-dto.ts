import { UserSimpleDto } from './user/user-simple-dto';

export class WaterTransferDto {
    OfferID: number;
    TransferDate: Date;
    TransferYear: number;
    AcreFeetTransferred: number;
    TransferringUser: UserSimpleDto;
    ReceivingUser: UserSimpleDto;
    Notes: string;
    ConfirmedByReceivingUser: boolean;
    ConfirmedByTransferringUser: boolean;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

