
export class OpenETSyncStatusTypeDto {
    OpenETSyncStatusTypeID: number;
    OpenETSyncStatusTypeName: string;
    OpenETSyncStatusTypeDisplayName: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

export enum OpenETSyncStatusTypeEnum {
    Nightly = 1,
    Finalized = 2,
    CurrentlyUpdating = 3
}
