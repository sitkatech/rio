export class PostingDetailedDto {
    PostingID: number;
    PostingDate: Date;
    PostingTypeID: number;
    PostingTypeDisplayName: string;
    PostingStatusID: number;
    PostingStatusDisplayName: string;

    PostedByUserID: number;
    PostedByAccountID: number;
    PostedByAccountName: string;
    PostedByFirstName: string;
    PostedByLastName: string;
    PostedByEmail: string;
    PostedByFullName: string;

    Quantity: number;
    AvailableQuantity: number;
    Price: number;
    NumberOfOffers: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

