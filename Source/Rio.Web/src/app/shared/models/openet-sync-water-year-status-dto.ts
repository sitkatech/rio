import { OpenETSyncStatusTypeDto } from './openet-sync-status-type-dto';

export class OpenETSyncWaterYearStatusDto {
    OpenETSyncWaterYearStatusID: number;
    WaterYear: number;
    OpenETSyncStatusType: OpenETSyncStatusTypeDto;
    LastUpdatedDate: Date

    constructor(obj?: any){
        Object.assign(this, obj);
    }
}


