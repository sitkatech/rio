
export class ParcelStatusDto {
    ParcelStatusID: number;
    ParcelStatusDisplayName: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
