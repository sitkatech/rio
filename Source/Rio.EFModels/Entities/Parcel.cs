﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public partial class Parcel
    {
        public static IEnumerable<ParcelDto> ListParcelsWithLandOwners(RioDbContext dbContext)
        {
            var parcels = dbContext.vParcelOwnership.Include(x => x.Parcel).Include(x=>x.User).AsNoTracking().Where(x =>
                x.RowNumber == 1 &&
                (x.EffectiveYear == null ||
                 x.EffectiveYear <=
                 DateTime.Now.Year)).Select(x=>x.AsParcelDto()).AsEnumerable();

            return parcels;
        }

        public static IEnumerable<ParcelDto> ListByUserID(RioDbContext dbContext, int userID)
        {
            var parcels = dbContext.vParcelOwnership.Include(x => x.Parcel).Include(x=>x.User).AsNoTracking().Where(x =>
                x.UserID == userID &&
                (x.EffectiveYear == null ||
                 x.EffectiveYear <=
                 DateTime.Now.Year)).ToList()
                .GroupBy(x=>x.ParcelID).Select(x=>x.OrderBy(x=>x.RowNumber).First())
                .Select(x => x.AsParcelDto()).AsEnumerable();

            return parcels;
        }

        public static ParcelDto GetByParcelID(RioDbContext dbContext, int parcelID)
        {
            var parcel = dbContext.Parcel
                .Include(x => x.UserParcel).ThenInclude(x => x.User)
                .AsNoTracking()
                .SingleOrDefault(x => x.ParcelID == parcelID);

            return parcel?.AsDto();
        }

        public static BoundingBoxDto GetBoundingBoxByParcelIDs(RioDbContext dbContext, List<int> parcelIDs)
        {
            var parcels = dbContext.Parcel
                .AsNoTracking()
                .Where(x => parcelIDs.Contains(x.ParcelID));

            var geometries = parcels.Select(x => x.ParcelGeometry).ToList();
            return new BoundingBoxDto(geometries);
        }
    }
}