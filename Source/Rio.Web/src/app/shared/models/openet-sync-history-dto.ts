import { OpenETSyncResultTypeDto } from "./openet-sync-result-type-dto";
import { WaterYearMonthDto } from './water-year-month-dto';


export class OpenETSyncHistoryDto {
    OpenETSyncHistoryID: number;
    OpenETSyncResultType: OpenETSyncResultTypeDto;
    WaterYearMonth: WaterYearMonthDto;
    CreateDate: Date;
    UpdateDate: Date;
    ErrorMessage: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}


