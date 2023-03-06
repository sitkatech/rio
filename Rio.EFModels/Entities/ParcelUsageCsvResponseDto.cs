using System.Collections.Generic;

namespace Rio.API.Models;

public class ParcelUsageCsvResponseDto
{
    public int TransactionCount { get; set; }
    public List<string> UnmatchedParcelNumbers { get; set; }

    public ParcelUsageCsvResponseDto(int transactionCount, List<string> unmatchedParcelNumbers)
    {
        TransactionCount = transactionCount;
        UnmatchedParcelNumbers = unmatchedParcelNumbers;
    }
}