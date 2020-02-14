export class ParcelMonthlyEvapotranspirationDto {
    ParcelID: number;
    ParcelNumber: string;
    WaterYear: number;
    WaterMonth: number;
    EvapotranspirationRate: number;
    OverriddenEvapotranspirationRate?: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}