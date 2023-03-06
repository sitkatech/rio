using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Rio.API.Models;

public class ParcelUsageCsvUpsertDto
{
    [Required]
    public IFormFile UploadedFile { get; set; }

    [Required]
    [RegularExpression(@"^\d{4}\-\d{1,2}\-\d{1,2}$",
        ErrorMessage = "Effective Date must be entered in YYYY-MM-DD format.")]
    public string EffectiveDate { get; set; }

    [Required]
    public string apnColumnName { get; set; }

    [Required]
    public string quantityColumnName { get; set; }
}