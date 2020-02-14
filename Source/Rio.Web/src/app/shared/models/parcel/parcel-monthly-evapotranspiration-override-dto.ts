export class ParcelMonthlyEvapotranspirationOverrideDto {
    ParcelID: number;
    ParcelNumber: string;
    WaterYear: number;
    WaterMonth: number;
    OverriddenEvapotranspirationRate: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}