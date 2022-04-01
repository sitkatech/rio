//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelStatus]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class ParcelStatus
    {
        public static readonly ParcelStatusActive Active = Rio.EFModels.Entities.ParcelStatusActive.Instance;
        public static readonly ParcelStatusInactive Inactive = Rio.EFModels.Entities.ParcelStatusInactive.Instance;

        public static readonly List<ParcelStatus> All;
        public static readonly List<ParcelStatusDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, ParcelStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, ParcelStatusDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ParcelStatus()
        {
            All = new List<ParcelStatus> { Active, Inactive };
            AllAsDto = new List<ParcelStatusDto> { Active.AsDto(), Inactive.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, ParcelStatus>(All.ToDictionary(x => x.ParcelStatusID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, ParcelStatusDto>(AllAsDto.ToDictionary(x => x.ParcelStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ParcelStatus(int parcelStatusID, string parcelStatusName, string parcelStatusDisplayName)
        {
            ParcelStatusID = parcelStatusID;
            ParcelStatusName = parcelStatusName;
            ParcelStatusDisplayName = parcelStatusDisplayName;
        }

        [Key]
        public int ParcelStatusID { get; private set; }
        public string ParcelStatusName { get; private set; }
        public string ParcelStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ParcelStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ParcelStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ParcelStatusID == ParcelStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ParcelStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ParcelStatusID;
        }

        public static bool operator ==(ParcelStatus left, ParcelStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ParcelStatus left, ParcelStatus right)
        {
            return !Equals(left, right);
        }

        public ParcelStatusEnum ToEnum => (ParcelStatusEnum)GetHashCode();

        public static ParcelStatus ToType(int enumValue)
        {
            return ToType((ParcelStatusEnum)enumValue);
        }

        public static ParcelStatus ToType(ParcelStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case ParcelStatusEnum.Active:
                    return Active;
                case ParcelStatusEnum.Inactive:
                    return Inactive;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum ParcelStatusEnum
    {
        Active = 1,
        Inactive = 2
    }

    public partial class ParcelStatusActive : ParcelStatus
    {
        private ParcelStatusActive(int parcelStatusID, string parcelStatusName, string parcelStatusDisplayName) : base(parcelStatusID, parcelStatusName, parcelStatusDisplayName) {}
        public static readonly ParcelStatusActive Instance = new ParcelStatusActive(1, @"Active", @"Active");
    }

    public partial class ParcelStatusInactive : ParcelStatus
    {
        private ParcelStatusInactive(int parcelStatusID, string parcelStatusName, string parcelStatusDisplayName) : base(parcelStatusID, parcelStatusName, parcelStatusDisplayName) {}
        public static readonly ParcelStatusInactive Instance = new ParcelStatusInactive(2, @"Inactive", @"Inactive");
    }
}