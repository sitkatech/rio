import { ParcelMonthlyEvapotranspirationDto } from './parcel-monthly-evapotranspiration-dto.1';

export class ParcelAllocationAndConsumptionDto {
    WaterYear: number;
    AcreFeetAllocated: number;
    MonthlyEvapotranspiration: Array<ParcelMonthlyEvapotranspirationDto>;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}