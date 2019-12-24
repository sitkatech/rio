import { AccountDto } from './account/account-dto';

export class WaterTransferRegistrationSimpleDto {
    WaterTransferTypeID: number;
    Account: AccountDto;
    WaterTransferRegistrationStatusID: number;
    StatusDate: Date;
    IsRegistered: boolean;
    IsCanceled: boolean;
    IsPending: boolean;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

