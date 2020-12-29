import { WaterYearDto } from "./water-year-dto";

export class WaterYearQuickOpenETHistoryDto {
    WaterYear: WaterYearDto;
    CurrentlySyncing: boolean;
    LastSuccessfulSync: Date

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}