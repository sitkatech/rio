using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Keyless]
    public partial class vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount
    {
        [StringLength(255)]
        [Unicode(false)]
        public string AccountName { get; set; }
        public bool? AccountAlreadyExists { get; set; }
        public int? WaterYearID { get; set; }
        [StringLength(8000)]
        [Unicode(false)]
        public string ExistingParcels { get; set; }
        [StringLength(8000)]
        [Unicode(false)]
        public string UpdatedParcels { get; set; }
    }
}
