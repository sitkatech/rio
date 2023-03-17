using System.Collections.Generic;

namespace Rio.API.Models;

public class ParcelUsageCSVResponseDto
{
    public List<string> UnmatchedParcelNumbers { get; set; }
    public int NullParcelNumberCount { get; set; }

    public ParcelUsageCSVResponseDto(List<string> unmatchedParcelNumbers, int nullParcelNumberCount)
    {
        UnmatchedParcelNumbers = unmatchedParcelNumbers;
        NullParcelNumberCount = nullParcelNumberCount;
    }
}