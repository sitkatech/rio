export class ParcelChangeOwnerDto {
    ParcelID: number;
    UserID: number;
    OwnerName: number;
    SaleDate: Date;
    EffectiveYear: number;
    Note: string;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
