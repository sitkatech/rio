export class ParcelLedgerCreateDto {
    ParcelID: number;
    EffectiveDate: Date;
    TransactionTypeID: number;
    TransactionAmount: number;
    WaterTypeID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
