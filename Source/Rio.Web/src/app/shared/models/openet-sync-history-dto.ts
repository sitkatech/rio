import { OpenETSyncResultTypeDto } from "./openet-sync-result-type-dto";


export class OpenETSyncHistoryDto {
    OpenETSyncHistoryID: number;
    OpenETSyncResultType: OpenETSyncResultTypeDto;
    WaterYear: WaterYearDto;
    CreateDate: Date;
    UpdateDate: Date;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

export class WaterYearDto {
    WaterYearID: number;
    Year: number;
    FinalizeDate: Date;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
