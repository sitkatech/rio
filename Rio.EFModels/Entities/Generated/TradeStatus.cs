using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("TradeStatus")]
    [Index(nameof(TradeStatusDisplayName), Name = "AK_TradeStatus_TradeStatusDisplayName", IsUnique = true)]
    [Index(nameof(TradeStatusName), Name = "AK_TradeStatus_TradeStatusName", IsUnique = true)]
    public partial class TradeStatus
    {
        public TradeStatus()
        {
            Trades = new HashSet<Trade>();
        }

        [Key]
        public int TradeStatusID { get; set; }
        [Required]
        [StringLength(100)]
        public string TradeStatusName { get; set; }
        [Required]
        [StringLength(100)]
        public string TradeStatusDisplayName { get; set; }

        [InverseProperty(nameof(Trade.TradeStatus))]
        public virtual ICollection<Trade> Trades { get; set; }
    }
}
