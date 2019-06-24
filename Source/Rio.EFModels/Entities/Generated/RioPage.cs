using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class RioPage
    {
        public RioPage()
        {
            RioPageImage = new HashSet<RioPageImage>();
        }

        [Column("RioPageID")]
        public int RioPageId { get; set; }
        [Column("RioPageTypeID")]
        public int RioPageTypeId { get; set; }
        public string RioPageContent { get; set; }

        [InverseProperty("RioPage")]
        public virtual ICollection<RioPageImage> RioPageImage { get; set; }
    }
}
