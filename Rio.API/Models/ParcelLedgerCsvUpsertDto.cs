using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Rio.API.Models;

public class ParcelLedgerCsvUpsertDto
{
    [Required]
    public IFormFile UploadedFile { get; set; }
    [Required]
    [RegularExpression(@"^\d{4}\-\d{1,2}\-\d{1,2}$",
        ErrorMessage = "Effective Date must be entered in YYYY-MM-DD format.")]
    public string EffectiveDate { get; set; }
    [Display(Name = "Supply Type")]
    [Required]
    public int? WaterTypeID { get; set; }
}