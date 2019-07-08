import { ParcelMonthlyEvapotranspirationDto } from './parcel-monthly-evapotranspiration-dto.1';

export class ParcelAllocationAndConsumptionDto {
    ParcelID: number;
    ParcelNumber: string;
    ParcelAreaInAcres: number;
    WaterYear: number;
    AcreFeetAllocated: number;
    MonthlyEvapotranspiration: Array<ParcelMonthlyEvapotranspirationDto>;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}