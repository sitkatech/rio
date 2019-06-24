export class Site {
    SiteID: number;
    Name: string;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
