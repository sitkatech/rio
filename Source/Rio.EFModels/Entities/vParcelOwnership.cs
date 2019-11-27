using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Rio.EFModels.Entities
{
    public partial class vParcelOwnership
    {
        [ForeignKey(nameof(ParcelID))]
        public virtual Parcel Parcel { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual User User { get; set; }
    }
}