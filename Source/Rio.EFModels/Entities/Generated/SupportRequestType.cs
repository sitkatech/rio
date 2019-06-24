using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class SupportRequestType
    {
        public SupportRequestType()
        {
            SupportRequestLog = new HashSet<SupportRequestLog>();
        }

        [Column("SupportRequestTypeID")]
        public int SupportRequestTypeId { get; set; }
        [Required]
        [StringLength(100)]
        public string SupportRequestTypeName { get; set; }
        [Required]
        [StringLength(100)]
        public string SupportRequestTypeDisplayName { get; set; }
        public int SupportRequestTypeSortOrder { get; set; }

        [InverseProperty("SupportRequestType")]
        public virtual ICollection<SupportRequestLog> SupportRequestLog { get; set; }
    }
}
