export class ParcelMonthlyEvapotranspirationDto {
    ParcelID: number;
    ParcelNumber: string;
    WaterYear: number;
    WaterMonth: number;
    EvapotranspirationRate: number;
    OverriddenEvapotranspirationRate?: number;
    IsEmpty: boolean;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}