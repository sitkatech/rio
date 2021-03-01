using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class OpenETGoogleBucketResponseEvapotranspirationData
    {
        [Key]
        public int OpenETGoogleBucketResponseEvapotranspirationDataID { get; set; }
        [Required]
        [StringLength(20)]
        public string ParcelNumber { get; set; }
        public int WaterMonth { get; set; }
        public int WaterYear { get; set; }
        [Column(TypeName = "decimal(20, 4)")]
        public decimal? EvapotranspirationRateInches { get; set; }
    }
}
