
export class WaterYearTransactionDto {
    WaterYear: number;
    AcreFeetPurchased: number;
    AcreFeetSold: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

