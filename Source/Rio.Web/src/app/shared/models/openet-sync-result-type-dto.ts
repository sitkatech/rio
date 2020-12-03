
export class OpenETSyncResultTypeDto {
    OpenETSyncResultTypeID: number;
    OpenETSyncResultTypeName: string;
    OpenETSyncResultTypeDisplayName: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

export enum OpenETSyncResultTypeEnum {
    InProgress = 1,
    Succeeded = 2,
    Failed = 3,
    NoNewData = 4,
    DataNotAvailable = 5
}
