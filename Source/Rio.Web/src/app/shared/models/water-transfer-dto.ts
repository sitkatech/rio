import { UserSimpleDto } from './user/user-simple-dto';

export class WaterTransferDto {
    WaterTransferID: number;
    OfferID: number;
    TransferDate: Date;
    TransferYear: number;
    AcreFeetTransferred: number;
    UnitPrice: number;
    Seller: UserSimpleDto;
    Buyer: UserSimpleDto;
    Notes: string;
    RegisteredByBuyer: boolean;
    RegisteredBySeller: boolean;
    TradeNumber: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

