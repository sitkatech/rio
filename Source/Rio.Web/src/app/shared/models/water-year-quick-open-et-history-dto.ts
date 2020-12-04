import { WaterYearDto } from './openet-sync-history-dto';

export class WaterYearQuickOpenETHistoryDto {
    WaterYear: WaterYearDto;
    CurrentlySyncing: boolean;
    LastSuccessfulSync: Date

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}