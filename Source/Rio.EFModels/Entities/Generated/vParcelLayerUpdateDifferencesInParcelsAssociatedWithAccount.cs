using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount
    {
        [StringLength(255)]
        public string AccountName { get; set; }
        public bool? AccountAlreadyExists { get; set; }
        public int? WaterYearID { get; set; }
        [StringLength(8000)]
        public string ExistingParcels { get; set; }
        [StringLength(8000)]
        public string UpdatedParcels { get; set; }
    }
}
