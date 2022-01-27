using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Rio.API.Models
{
    public class ParcelLedgerCreateCSVUploadDto
    {
        [Required]
        public IFormFile UploadedFile { get; set; }
        [Required]
        public DateTime EffectiveDate { get; set; }
        public String EffectiveDateString { get; set; }
        [Required]
        public int WaterTypeID { get; set; }
    }
}