export class WaterTransferRegistrationParcelDto {
    ParcelID: number;
    ParcelNumber: string;
    AcreFeetTransferred: number;
    ParcelAreaInAcres: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}