export class CustomRichTextDto{
    public CustomRichTextContent: string;
    public IsEmptyContent: boolean;

    constructor(obj?: any){
        Object.assign(this, obj);
    }
}