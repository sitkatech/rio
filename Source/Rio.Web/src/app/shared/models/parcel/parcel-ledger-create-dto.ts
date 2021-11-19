export class ParcelLedgerCreateDto {
    ParcelID: number;
    ParcelNumber: string;
    EffectiveDate: Date;
    TransactionTypeID: number;
    TransactionAmount: number;
    WaterTypeID: number;
    UserComment: string;
    IsWithdrawal: boolean;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
