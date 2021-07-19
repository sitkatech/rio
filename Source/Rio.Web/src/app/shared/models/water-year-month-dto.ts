import { WaterYearDto } from "./water-year-dto";


export class WaterYearMonthDto {
    WaterYearMonthID: number;
    WaterYear: WaterYearDto;
    Month: number;
    FinalizeDate: Date;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
