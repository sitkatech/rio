import * as internal from "stream";

export class ParcelLedgerDto {
    ParcelID: number;
    TransactionDate: Date;
    TransactionTypeID: number;
    TransactionAmount: number;
    TransactionDescription: string;
    ParcelLedgerID: number;
    WaterYear: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
