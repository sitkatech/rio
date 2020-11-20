export class ParcelOwnershipDto {
    OwnerName: string;
    OwnerAccountID: number;
    EffectiveYear: number;
    SaleDate: string;
    Note: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}