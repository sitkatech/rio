using CsvHelper.Configuration;

namespace Qanat.EFModels.Entities;

public class ParcelTransactionCSV
{
    public string APN { get; set; }
    public double? Quantity { get; set; }
}

public sealed class ParcelTransactionCSVMap : ClassMap<ParcelTransactionCSV>
{
    public ParcelTransactionCSVMap(string apnColumnName, string quantityColumnName)
    {
        Map(m => m.APN).Name(apnColumnName);
        Map(m => m.Quantity).Name(quantityColumnName);
    }
}