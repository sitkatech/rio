using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("WaterType")]
    [Index("WaterTypeName", Name = "AK_WaterType_WaterTypeName", IsUnique = true)]
    public partial class WaterType
    {
        public WaterType()
        {
            ParcelLedgers = new HashSet<ParcelLedger>();
        }

        [Key]
        public int WaterTypeID { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string WaterTypeName { get; set; }
        public bool IsAppliedProportionally { get; set; }
        [Unicode(false)]
        public string WaterTypeDefinition { get; set; }
        public bool IsSourcedFromApi { get; set; }
        public int SortOrder { get; set; }

        [InverseProperty("WaterType")]
        public virtual ICollection<ParcelLedger> ParcelLedgers { get; set; }
    }
}
