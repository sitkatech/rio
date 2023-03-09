using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Rio.API.Models;

public class CsvUpsertDto
{
    [Required]
    public IFormFile UploadedFile { get; set; }
}