export class ParcelChangeOwnerDto {
    ParcelID: number;
    AccountID: number;
    OwnerName: number;
    SaleDate: Date;
    EffectiveYear: number;
    Note: string;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
