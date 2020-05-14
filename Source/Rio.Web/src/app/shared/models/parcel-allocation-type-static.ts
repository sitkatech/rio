export class ParcelAllocationTypeStatic {
    static readonly ProjectWater = new ParcelAllocationTypeStatic(
        "Project Water",
        1
    )

    static readonly Reconciliation = new ParcelAllocationTypeStatic(
        "Reconciliation",
        2
    )

    static readonly NativeYield = new ParcelAllocationTypeStatic(
        "Native Yield",
        3
    )

    static readonly StoredWater = new ParcelAllocationTypeStatic(
        "Stored Water",
        4
    )

    private constructor(private readonly key:string, 
        public readonly id:number) {}

    toString() {
        return this.key;
    }
}

