export class WaterTransferParcelDto {
    ParcelID: number;
    ParcelNumber: string;
    AcreFeetTransferred: number;
    WaterTransferTypeID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}