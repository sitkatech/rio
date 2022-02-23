export class ValidationError {
    constructor(public entity?: string,
                public invalidValue?: string,
                public property?: string,
                public message?: string) {
    }
}
