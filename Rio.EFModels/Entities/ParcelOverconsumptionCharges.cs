using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities;

public static class AccountOverconsumptionCharges {

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
        var existingAccountOverconsumptionCharges = dbContext.AccountOverconsumptionCharges
            .Where(x => x.WaterYearID == waterYear.WaterYearID).ToList();
        dbContext.AccountOverconsumptionCharges.RemoveRange(existingAccountOverconsumptionCharges);

        var parcelLedgersByParcelID = dbContext.ParcelLedgers.AsNoTracking()
            .Where(x => x.EffectiveDate.Year == waterYear.Year)
            .ToLookup(x => x.ParcelID);

        var accountGroups = dbContext.AccountParcelWaterYears
            .Where(x => x.WaterYearID == waterYear.WaterYearID).ToList()
            .GroupBy(x => x.AccountID);

        var accountOverconsumptionCharges = new List<AccountOverconsumptionCharge>();
        foreach (var accountGroup in accountGroups)
        {
            var totalRemaining = accountGroup.Sum(x => 
                parcelLedgersByParcelID[x.ParcelID].Sum(y => y.TransactionAmount));

            var accountOverconsumptionCharge = new AccountOverconsumptionCharge
            {
                AccountID = accountGroup.Key,
                WaterYearID = waterYear.WaterYearID
            };

            CalculateOverconsumptionCharge(accountOverconsumptionCharge, totalRemaining, waterYear.OverconsumptionRate);
            accountOverconsumptionCharges.Add(accountOverconsumptionCharge);
        }
        
        dbContext.AccountOverconsumptionCharges.AddRange(accountOverconsumptionCharges);
        dbContext.SaveChanges();
    }

    public static void UpdateByYearAndParcelID(RioDbContext dbContext, int year, int parcelID)
    {
        var waterYear = WaterYear.GetByYear(dbContext, year);
        if (waterYear == null || waterYear.OverconsumptionRate == 0) return;

        var accountParcelWaterYear = AccountParcelWaterYear.GetByParcelIDAndWaterYearID(dbContext, parcelID, waterYear.WaterYearID);
        if (accountParcelWaterYear == null) return;

        var accountOverconsumptionCharge = dbContext.AccountOverconsumptionCharges
            .SingleOrDefault(x => x.AccountID == accountParcelWaterYear.AccountID && x.WaterYearID == waterYear.WaterYearID);
        
        // if record doesn't exist, overconsumption charge hasn't been set for the associated water year
        if (accountOverconsumptionCharge == null) return;

        var totalRemaining = dbContext.ParcelLedgers.AsNoTracking()
            .Where(x => x.EffectiveDate.Year == waterYear.WaterYearID && x.ParcelID == parcelID)
            .Sum(x => x.TransactionAmount);
        

        CalculateOverconsumptionCharge(accountOverconsumptionCharge, totalRemaining, waterYear.OverconsumptionRate);
        dbContext.SaveChanges();
    }

    private static void CalculateOverconsumptionCharge(AccountOverconsumptionCharge accountOverconsumptionCharge, decimal totalRemaining, decimal overconsumptionRate)
    {
        // only populate the overconsumption amount if total remaining is less than 0
        var overconsumptionAmount = totalRemaining < 0 ? (-1 * totalRemaining) : 0;

        accountOverconsumptionCharge.OverconsumptionAmount = overconsumptionAmount;
        accountOverconsumptionCharge.OverconsumptionCharge = overconsumptionAmount * overconsumptionRate;
    }
}