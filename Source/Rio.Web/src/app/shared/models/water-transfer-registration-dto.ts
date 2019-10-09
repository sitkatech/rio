import { WaterTransferRegistrationParcelDto } from './water-transfer-registration-parcel-dto';

export class WaterTransferRegistrationDto {
    WaterTransferTypeID: number;
    UserID: number;
    WaterTransferRegistrationParcels: Array<WaterTransferRegistrationParcelDto>;
    DateRegistered: Date;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

