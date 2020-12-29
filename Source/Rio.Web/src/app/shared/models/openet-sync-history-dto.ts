import { OpenETSyncResultTypeDto } from "./openet-sync-result-type-dto";
import { WaterYearDto } from './water-year-dto';


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


