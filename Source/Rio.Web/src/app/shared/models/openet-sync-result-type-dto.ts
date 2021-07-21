
export class OpenETSyncResultTypeDto {
    OpenETSyncResultTypeID: number;
    OpenETSyncResultTypeName: string;
    OpenETSyncResultTypeDisplayName: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
