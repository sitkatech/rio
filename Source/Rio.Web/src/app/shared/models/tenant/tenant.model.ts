import { TemplateModule } from '../template-module/template-module.model';

export class Tenant {
    Name: string;
    SubDomain: string;
    Modules: TemplateModule[];

    ThemeName: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
