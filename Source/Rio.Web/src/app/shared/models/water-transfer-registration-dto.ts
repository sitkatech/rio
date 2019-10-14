import { WaterTransferRegistrationParcelDto } from './water-transfer-registration-parcel-dto';

export class WaterTransferRegistrationDto {
    WaterTransferTypeID: number;
    UserID: number;
    WaterTransferRegistrationStatusID: number;
    StatusDate: Date;
    WaterTransferRegistrationParcels: Array<WaterTransferRegistrationParcelDto>;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

