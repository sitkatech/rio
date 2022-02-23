export class FeatureClassInfoDto {
    LayerName: string;
    FeatureType: string;
    FeatureCount: number;
    Columns: string[];

    constructor(obj?: any){
        Object.assign(this, obj);
    }
}