using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class vParcelOwnership
    {
        [ForeignKey(nameof(ParcelID))]
        public virtual Parcel Parcel { get; set; }

        [ForeignKey(nameof(AccountID))]
        public virtual Account Account { get; set; }
        
        [ForeignKey(nameof(WaterYearID))]
        public virtual WaterYear WaterYear { get; set; }
    }
}