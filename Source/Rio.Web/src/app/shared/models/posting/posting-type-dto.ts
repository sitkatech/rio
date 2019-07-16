export class PostingTypeDto {
    PostingTypeID: number;
    PostingTypeName: string;
    PostingTypeDisplayName: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}