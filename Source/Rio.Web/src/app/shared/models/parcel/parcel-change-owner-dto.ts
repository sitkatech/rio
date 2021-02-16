export class ParcelChangeOwnerDto {
    ParcelID: number;
    AccountID: number;
    EffectiveWaterYearID: number;
    ApplyToSubsequentYears: boolean;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
