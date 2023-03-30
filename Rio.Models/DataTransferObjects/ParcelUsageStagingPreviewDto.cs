using System;
using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects;

public class ParcelUsageStagingPreviewDto
{
    public List<ParcelUsageStagingSimpleDto> StagedParcelUsages { get; set; }
    public List<string> ParcelNumbersWithoutStagedUsages { get; set; }
    public int NullParcelNumberCount { get; set; }
}