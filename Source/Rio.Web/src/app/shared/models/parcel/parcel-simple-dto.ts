export class ParcelSimpleDto {
    ParcelID: number;
    ParcelNumber: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}