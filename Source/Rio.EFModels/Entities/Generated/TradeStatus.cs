using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class TradeStatus
    {
        public TradeStatus()
        {
            Trade = new HashSet<Trade>();
        }

        [Key]
        public int TradeStatusID { get; set; }
        [Required]
        [StringLength(100)]
        public string TradeStatusName { get; set; }
        [Required]
        [StringLength(100)]
        public string TradeStatusDisplayName { get; set; }

        [InverseProperty("TradeStatus")]
        public virtual ICollection<Trade> Trade { get; set; }
    }
}
