using System;

namespace Rio.EFModels.Entities
{
    public partial class ParcelLedger
    {
        public int WaterYear => EffectiveDate.Year;
        public int WaterMonth => EffectiveDate.Month;
    }
}