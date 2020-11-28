import { OpenETSyncResultTypeDto } from "./openet-sync-result-type-dto";


export class OpenETSyncHistoryDto {
    OpenETSyncHistoryID: number;
    OpenETSyncResultType: OpenETSyncResultTypeDto;
    YearsInUpdateSeparatedByComma: string;
    LastUpdatedDate: Date;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
