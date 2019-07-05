export class ParcelMonthlyEvapotranspirationDto {
    WaterYear: number;
    WaterMonth: number;
    EvapotranspirationRate: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}