using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class RioPageImage
    {
        [Column("RioPageImageID")]
        public int RioPageImageId { get; set; }
        [Column("RioPageID")]
        public int RioPageId { get; set; }
        [Column("FileResourceID")]
        public int FileResourceId { get; set; }

        [ForeignKey("RioPageId")]
        [InverseProperty("RioPageImage")]
        public virtual RioPage RioPage { get; set; }
        [ForeignKey("FileResourceId")]
        [InverseProperty("RioPageImage")]
        public virtual FileResource FileResource { get; set; }
    }
}
