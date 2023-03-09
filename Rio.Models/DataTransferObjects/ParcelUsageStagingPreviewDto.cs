using System;
using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects;

public class ParcelUsageStagingPreviewDto
{
    public List<ParcelUsageStagingSimpleDto> StagedParcelUsages { get; set; }
    public List<string> ParcelNumbersWithoutStagedUsages { get; set; }

    public ParcelUsageStagingPreviewDto(List<ParcelUsageStagingSimpleDto> stagedParcelUsages,
        List<string> parcelNumbersWithoutStagedUsages)
    {
        StagedParcelUsages = stagedParcelUsages;
        ParcelNumbersWithoutStagedUsages = parcelNumbersWithoutStagedUsages;
    }
}