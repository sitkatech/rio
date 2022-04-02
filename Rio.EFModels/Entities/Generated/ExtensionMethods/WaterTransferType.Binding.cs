//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTransferType]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class WaterTransferType
    {
        public static readonly WaterTransferTypeBuying Buying = Rio.EFModels.Entities.WaterTransferTypeBuying.Instance;
        public static readonly WaterTransferTypeSelling Selling = Rio.EFModels.Entities.WaterTransferTypeSelling.Instance;

        public static readonly List<WaterTransferType> All;
        public static readonly List<WaterTransferTypeDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, WaterTransferType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, WaterTransferTypeDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static WaterTransferType()
        {
            All = new List<WaterTransferType> { Buying, Selling };
            AllAsDto = new List<WaterTransferTypeDto> { Buying.AsDto(), Selling.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, WaterTransferType>(All.ToDictionary(x => x.WaterTransferTypeID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, WaterTransferTypeDto>(AllAsDto.ToDictionary(x => x.WaterTransferTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected WaterTransferType(int waterTransferTypeID, string waterTransferTypeName, string waterTransferTypeDisplayName)
        {
            WaterTransferTypeID = waterTransferTypeID;
            WaterTransferTypeName = waterTransferTypeName;
            WaterTransferTypeDisplayName = waterTransferTypeDisplayName;
        }

        [Key]
        public int WaterTransferTypeID { get; private set; }
        public string WaterTransferTypeName { get; private set; }
        public string WaterTransferTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return WaterTransferTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(WaterTransferType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.WaterTransferTypeID == WaterTransferTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as WaterTransferType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return WaterTransferTypeID;
        }

        public static bool operator ==(WaterTransferType left, WaterTransferType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WaterTransferType left, WaterTransferType right)
        {
            return !Equals(left, right);
        }

        public WaterTransferTypeEnum ToEnum => (WaterTransferTypeEnum)GetHashCode();

        public static WaterTransferType ToType(int enumValue)
        {
            return ToType((WaterTransferTypeEnum)enumValue);
        }

        public static WaterTransferType ToType(WaterTransferTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case WaterTransferTypeEnum.Buying:
                    return Buying;
                case WaterTransferTypeEnum.Selling:
                    return Selling;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum WaterTransferTypeEnum
    {
        Buying = 1,
        Selling = 2
    }

    public partial class WaterTransferTypeBuying : WaterTransferType
    {
        private WaterTransferTypeBuying(int waterTransferTypeID, string waterTransferTypeName, string waterTransferTypeDisplayName) : base(waterTransferTypeID, waterTransferTypeName, waterTransferTypeDisplayName) {}
        public static readonly WaterTransferTypeBuying Instance = new WaterTransferTypeBuying(1, @"Buying", @"Buying");
    }

    public partial class WaterTransferTypeSelling : WaterTransferType
    {
        private WaterTransferTypeSelling(int waterTransferTypeID, string waterTransferTypeName, string waterTransferTypeDisplayName) : base(waterTransferTypeID, waterTransferTypeName, waterTransferTypeDisplayName) {}
        public static readonly WaterTransferTypeSelling Instance = new WaterTransferTypeSelling(2, @"Selling", @"Selling");
    }
}