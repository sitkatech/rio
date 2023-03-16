using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities;

public static class ParcelOverconsumptionCharges {

    public static void UpdateByYear(RioDbContext dbContext, int year)
    {
        var waterYear = WaterYear.GetByYear(dbContext, year);
        if (waterYear == null)
        {
            return;
        }

        UpdateByWaterYear(dbContext, waterYear);
    }

    public static void UpdateByWaterYear(RioDbContext dbContext, WaterYear waterYear)
    {
        var existingParcelOverconsumptions = dbContext.ParcelOverconsumptionCharges
            .Where(x => x.WaterYearID == waterYear.WaterYearID).ToList();
        dbContext.ParcelOverconsumptionCharges.RemoveRange(existingParcelOverconsumptions);

        var parcelGroups = dbContext.ParcelLedgers.AsNoTracking()
            .Where(x => x.TransactionDate.Year == waterYear.Year).ToList()
            .GroupBy(x => x.ParcelID);

        var newParcelOverconsumptions = new List<ParcelOverconsumptionCharge>();
        foreach (var parcelGroup in parcelGroups)
        {
            var parcelOverconsumptionCharge = new ParcelOverconsumptionCharge();
            parcelOverconsumptionCharge.ParcelID = parcelGroup.Key;
            parcelOverconsumptionCharge.WaterYearID = waterYear.WaterYearID;

            UpdateParcelOverconsumptionCharge(parcelOverconsumptionCharge, new List<ParcelLedger>(parcelGroup), waterYear.OverconsumptionRate);
            newParcelOverconsumptions.Add(parcelOverconsumptionCharge);
        }

        dbContext.AddRange(newParcelOverconsumptions);
        dbContext.SaveChanges();
    }

    public static void UpdateByYearAndParcelID(RioDbContext dbContext, int year, int parcelID)
    {
        var waterYear = WaterYear.GetByYear(dbContext, year);
        if (waterYear == null || waterYear.OverconsumptionRate == 0)
        {
            return;
        }

        var parcelOverconsumptionCharge = dbContext.ParcelOverconsumptionCharges
            .SingleOrDefault(x => x.WaterYearID == waterYear.WaterYearID && x.ParcelID == parcelID);
        if (parcelOverconsumptionCharge == null)
        {
            // if record doesn't exist, overconsumption charge hasn't been set for the associated water year
            return;
        }

        var parcelLedgers = dbContext.ParcelLedgers.AsNoTracking()
            .Where(x => x.TransactionDate.Year == waterYear.WaterYearID && x.ParcelID == parcelID).ToList();

        UpdateParcelOverconsumptionCharge(parcelOverconsumptionCharge, parcelLedgers, waterYear.OverconsumptionRate);
        dbContext.SaveChanges();
    }

    private static void UpdateParcelOverconsumptionCharge(ParcelOverconsumptionCharge parcelOverconsumptionCharge, List<ParcelLedger> parcelLedgers, decimal overconsumptionRate)
    {
        var totalRemaining = parcelLedgers.Sum(x => x.TransactionAmount);

        // only populate the overconsumption amount if total remaining is less than 0
        var overconsumptionAmount = totalRemaining < 0 ? (-1 * totalRemaining) : 0;

        parcelOverconsumptionCharge.OverconsumptionAmount = overconsumptionAmount;
        parcelOverconsumptionCharge.OverconsumptionCharge = overconsumptionAmount * overconsumptionRate;
    }
}