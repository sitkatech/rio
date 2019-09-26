export class WaterTransferRegistrationParcelDto {
    ParcelID: number;
    ParcelNumber: string;
    AcreFeetTransferred: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}