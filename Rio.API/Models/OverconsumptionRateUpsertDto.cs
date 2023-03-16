using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rio.API.Models;

public class OverconsumptionRateUpsertDto
{
    [Required]
    [DisplayName("Water Year")]
    public int? WaterYearID { get; set; }

    [Required]
    [DisplayName("Overconsumption Rate")]
    public decimal? OverconsumptionRate { get; set; }
}