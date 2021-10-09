import * as internal from "stream";

export class ParcelLedgerDto {
    ParcelID: number;
    TransactionDate: Date;
    EffectiveDate: Date;
    TransactionTypeID: number;
    TransactionAmount: number;
    TransactionDescription: string;
    ParcelLedgerID: number;
    WaterYear: number;
    WaterMonth: number;
    WaterTypeID: number;
    TransactionTypeDisplayName: string;
    WaterTypeDisplayName: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
