//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelLedgerEntrySourceType]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class ParcelLedgerEntrySourceType
    {
        public static readonly ParcelLedgerEntrySourceTypeManual Manual = Rio.EFModels.Entities.ParcelLedgerEntrySourceTypeManual.Instance;
        public static readonly ParcelLedgerEntrySourceTypeOpenET OpenET = Rio.EFModels.Entities.ParcelLedgerEntrySourceTypeOpenET.Instance;
        public static readonly ParcelLedgerEntrySourceTypeTrade Trade = Rio.EFModels.Entities.ParcelLedgerEntrySourceTypeTrade.Instance;

        public static readonly List<ParcelLedgerEntrySourceType> All;
        public static readonly List<ParcelLedgerEntrySourceTypeDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, ParcelLedgerEntrySourceType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, ParcelLedgerEntrySourceTypeDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ParcelLedgerEntrySourceType()
        {
            All = new List<ParcelLedgerEntrySourceType> { Manual, OpenET, Trade };
            AllAsDto = new List<ParcelLedgerEntrySourceTypeDto> { Manual.AsDto(), OpenET.AsDto(), Trade.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, ParcelLedgerEntrySourceType>(All.ToDictionary(x => x.ParcelLedgerEntrySourceTypeID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, ParcelLedgerEntrySourceTypeDto>(AllAsDto.ToDictionary(x => x.ParcelLedgerEntrySourceTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ParcelLedgerEntrySourceType(int parcelLedgerEntrySourceTypeID, string parcelLedgerEntrySourceTypeName, string parcelLedgerEntrySourceTypeDisplayName)
        {
            ParcelLedgerEntrySourceTypeID = parcelLedgerEntrySourceTypeID;
            ParcelLedgerEntrySourceTypeName = parcelLedgerEntrySourceTypeName;
            ParcelLedgerEntrySourceTypeDisplayName = parcelLedgerEntrySourceTypeDisplayName;
        }

        [Key]
        public int ParcelLedgerEntrySourceTypeID { get; private set; }
        public string ParcelLedgerEntrySourceTypeName { get; private set; }
        public string ParcelLedgerEntrySourceTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ParcelLedgerEntrySourceTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ParcelLedgerEntrySourceType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ParcelLedgerEntrySourceTypeID == ParcelLedgerEntrySourceTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ParcelLedgerEntrySourceType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ParcelLedgerEntrySourceTypeID;
        }

        public static bool operator ==(ParcelLedgerEntrySourceType left, ParcelLedgerEntrySourceType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ParcelLedgerEntrySourceType left, ParcelLedgerEntrySourceType right)
        {
            return !Equals(left, right);
        }

        public ParcelLedgerEntrySourceTypeEnum ToEnum => (ParcelLedgerEntrySourceTypeEnum)GetHashCode();

        public static ParcelLedgerEntrySourceType ToType(int enumValue)
        {
            return ToType((ParcelLedgerEntrySourceTypeEnum)enumValue);
        }

        public static ParcelLedgerEntrySourceType ToType(ParcelLedgerEntrySourceTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case ParcelLedgerEntrySourceTypeEnum.Manual:
                    return Manual;
                case ParcelLedgerEntrySourceTypeEnum.OpenET:
                    return OpenET;
                case ParcelLedgerEntrySourceTypeEnum.Trade:
                    return Trade;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum ParcelLedgerEntrySourceTypeEnum
    {
        Manual = 1,
        OpenET = 2,
        Trade = 3
    }

    public partial class ParcelLedgerEntrySourceTypeManual : ParcelLedgerEntrySourceType
    {
        private ParcelLedgerEntrySourceTypeManual(int parcelLedgerEntrySourceTypeID, string parcelLedgerEntrySourceTypeName, string parcelLedgerEntrySourceTypeDisplayName) : base(parcelLedgerEntrySourceTypeID, parcelLedgerEntrySourceTypeName, parcelLedgerEntrySourceTypeDisplayName) {}
        public static readonly ParcelLedgerEntrySourceTypeManual Instance = new ParcelLedgerEntrySourceTypeManual(1, @"Manual", @"Manual");
    }

    public partial class ParcelLedgerEntrySourceTypeOpenET : ParcelLedgerEntrySourceType
    {
        private ParcelLedgerEntrySourceTypeOpenET(int parcelLedgerEntrySourceTypeID, string parcelLedgerEntrySourceTypeName, string parcelLedgerEntrySourceTypeDisplayName) : base(parcelLedgerEntrySourceTypeID, parcelLedgerEntrySourceTypeName, parcelLedgerEntrySourceTypeDisplayName) {}
        public static readonly ParcelLedgerEntrySourceTypeOpenET Instance = new ParcelLedgerEntrySourceTypeOpenET(2, @"OpenET", @"OpenET");
    }

    public partial class ParcelLedgerEntrySourceTypeTrade : ParcelLedgerEntrySourceType
    {
        private ParcelLedgerEntrySourceTypeTrade(int parcelLedgerEntrySourceTypeID, string parcelLedgerEntrySourceTypeName, string parcelLedgerEntrySourceTypeDisplayName) : base(parcelLedgerEntrySourceTypeID, parcelLedgerEntrySourceTypeName, parcelLedgerEntrySourceTypeDisplayName) {}
        public static readonly ParcelLedgerEntrySourceTypeTrade Instance = new ParcelLedgerEntrySourceTypeTrade(3, @"Trade", @"Trade");
    }
}