using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class RioPageImage
    {
        public int RioPageImageID { get; set; }
        public int RioPageID { get; set; }
        public int FileResourceID { get; set; }

        [ForeignKey("FileResourceID")]
        [InverseProperty("RioPageImage")]
        public virtual FileResource FileResource { get; set; }
        [ForeignKey("RioPageID")]
        [InverseProperty("RioPageImage")]
        public virtual RioPage RioPage { get; set; }
    }
}
