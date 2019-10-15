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

        [Key]
        public int RioPageID { get; set; }
        public int RioPageTypeID { get; set; }
        public string RioPageContent { get; set; }

        [InverseProperty("RioPage")]
        public virtual ICollection<RioPageImage> RioPageImage { get; set; }
    }
}
