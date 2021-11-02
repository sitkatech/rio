export class ParcelLedgerCreateDto {
    ParcelID: number;
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
