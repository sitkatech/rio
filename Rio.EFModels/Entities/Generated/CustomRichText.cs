﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("CustomRichText")]
    public partial class CustomRichText
    {
        [Key]
        public int CustomRichTextID { get; set; }
        public int CustomRichTextTypeID { get; set; }
        public string CustomRichTextContent { get; set; }
    }
}
