import { UserSimpleDto } from '../user/user-simple-dto';

export class ParcelWithWaterUsageDto {
    ParcelID: number;
    ParcelNumber: string;
    ParcelAreaInAcres: number;
    LandOwner: UserSimpleDto;
    WaterUsageFor2016: number;
    WaterUsageFor2017: number;
    WaterUsageFor2018: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}