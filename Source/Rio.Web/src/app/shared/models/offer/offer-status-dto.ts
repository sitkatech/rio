export class OfferStatusDto {
    OfferStatusID: number;
    OfferStatusName: string;
    OfferStatusDisplayName: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}