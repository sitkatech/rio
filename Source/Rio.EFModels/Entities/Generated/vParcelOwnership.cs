using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class vParcelOwnership
    {
        public int ParcelID { get; set; }
        public int WaterYearID { get; set; }
        public int? AccountID { get; set; }
    }
}
