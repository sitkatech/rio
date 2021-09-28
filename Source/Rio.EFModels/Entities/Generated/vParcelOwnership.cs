using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Keyless]
    public partial class vParcelOwnership
    {
        public int ParcelID { get; set; }
        public int WaterYearID { get; set; }
        public int? AccountID { get; set; }
    }
}
