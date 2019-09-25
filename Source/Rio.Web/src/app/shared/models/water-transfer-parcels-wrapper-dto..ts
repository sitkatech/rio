import { WaterTransferParcelDto } from './water-transfer-parcel-dto';

export class WaterTransferParcelsWrapperDto {
    WaterTransferParcels: Array<WaterTransferParcelDto>;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}