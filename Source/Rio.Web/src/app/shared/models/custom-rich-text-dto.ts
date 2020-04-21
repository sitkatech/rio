export class CustomRichTextDto{
    public CustomRichTextContent: string;

    constructor(obj?: any){
        Object.assign(this, obj);
    }
}