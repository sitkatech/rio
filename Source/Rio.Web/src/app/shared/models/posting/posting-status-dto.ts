export class PostingStatusDto {
    PostingStatusID: number;
    PostingStatusName: string;
    PostingStatusDisplayName: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}