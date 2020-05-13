import { AccountSimpleDto } from '../account/account-simple-dto';

export class UserDetailedDto {
    UserID: number;
    UserGuid: string;

    FirstName: string;
    LastName: string;
    FullName: string;

    Email: string;
    Phone: string;
    LoginName: string;

    RoleID: number;
    RoleDisplayName: string;

    HasActiveTrades: boolean;
    AcreFeetOfWaterPurchased: number;
    AcreFeetOfWaterSold: number;
    ReceiveSupportEmails: boolean;
    IsActive: boolean;

    AssociatedAccounts: Array<AccountSimpleDto>

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

