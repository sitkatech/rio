export class TemplateModule {
    Name: string;
    RoutePath: string;
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
