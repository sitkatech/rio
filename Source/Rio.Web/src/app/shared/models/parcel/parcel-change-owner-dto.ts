export class ParcelChangeOwnerDto {
    ParcelID: number;
    UserID: number;
    OwnerName: number;
    SaleDate: Date;
    EffectiveYear: number;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
