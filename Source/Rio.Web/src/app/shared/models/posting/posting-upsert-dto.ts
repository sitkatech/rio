export class PostingUpsertDto {
    PostingTypeID: number;
    Price: number;

    Quantity: number;
    PostingDescription: string;
    CreateAccountID: number;
    CreateUserID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}