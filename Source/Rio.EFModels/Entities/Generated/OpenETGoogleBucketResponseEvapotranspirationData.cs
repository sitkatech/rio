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
        public int ParcelID { get; set; }
        public int WaterMonth { get; set; }
        public int WaterYear { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal EvapotranspirationRate { get; set; }
    }
}
