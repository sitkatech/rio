namespace Rio.Models.DataTransferObjects
{
    public class WaterTypeDto
    {
        public int WaterTypeID { get; set; }
        public string WaterTypeName { get; set; }
        public WaterTypeApplicationTypeEnum IsAppliedProportionally { get; set; }
        public string WaterTypeDefinition { get; set; }
        public int SortOrder { get; set; }
    }

    public enum WaterTypeApplicationTypeEnum
    {
        Spreadsheet = 0,
        Proportionally = 1,
        Api = 2
    }
}
