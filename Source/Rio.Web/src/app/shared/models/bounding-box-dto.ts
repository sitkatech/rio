export class BoundingBoxDto {
    Left : number;
    Bottom : number;
    Right : number;
    Top : number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
