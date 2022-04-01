//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTransferRegistrationStatus]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class WaterTransferRegistrationStatus
    {
        public static readonly WaterTransferRegistrationStatusPending Pending = Rio.EFModels.Entities.WaterTransferRegistrationStatusPending.Instance;
        public static readonly WaterTransferRegistrationStatusRegistered Registered = Rio.EFModels.Entities.WaterTransferRegistrationStatusRegistered.Instance;
        public static readonly WaterTransferRegistrationStatusCanceled Canceled = Rio.EFModels.Entities.WaterTransferRegistrationStatusCanceled.Instance;

        public static readonly List<WaterTransferRegistrationStatus> All;
        public static readonly List<WaterTransferRegistrationStatusDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, WaterTransferRegistrationStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, WaterTransferRegistrationStatusDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static WaterTransferRegistrationStatus()
        {
            All = new List<WaterTransferRegistrationStatus> { Pending, Registered, Canceled };
            AllAsDto = new List<WaterTransferRegistrationStatusDto> { Pending.AsDto(), Registered.AsDto(), Canceled.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, WaterTransferRegistrationStatus>(All.ToDictionary(x => x.WaterTransferRegistrationStatusID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, WaterTransferRegistrationStatusDto>(AllAsDto.ToDictionary(x => x.WaterTransferRegistrationStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected WaterTransferRegistrationStatus(int waterTransferRegistrationStatusID, string waterTransferRegistrationStatusName, string waterTransferRegistrationStatusDisplayName)
        {
            WaterTransferRegistrationStatusID = waterTransferRegistrationStatusID;
            WaterTransferRegistrationStatusName = waterTransferRegistrationStatusName;
            WaterTransferRegistrationStatusDisplayName = waterTransferRegistrationStatusDisplayName;
        }

        [Key]
        public int WaterTransferRegistrationStatusID { get; private set; }
        public string WaterTransferRegistrationStatusName { get; private set; }
        public string WaterTransferRegistrationStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return WaterTransferRegistrationStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(WaterTransferRegistrationStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.WaterTransferRegistrationStatusID == WaterTransferRegistrationStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as WaterTransferRegistrationStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return WaterTransferRegistrationStatusID;
        }

        public static bool operator ==(WaterTransferRegistrationStatus left, WaterTransferRegistrationStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WaterTransferRegistrationStatus left, WaterTransferRegistrationStatus right)
        {
            return !Equals(left, right);
        }

        public WaterTransferRegistrationStatusEnum ToEnum => (WaterTransferRegistrationStatusEnum)GetHashCode();

        public static WaterTransferRegistrationStatus ToType(int enumValue)
        {
            return ToType((WaterTransferRegistrationStatusEnum)enumValue);
        }

        public static WaterTransferRegistrationStatus ToType(WaterTransferRegistrationStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case WaterTransferRegistrationStatusEnum.Canceled:
                    return Canceled;
                case WaterTransferRegistrationStatusEnum.Pending:
                    return Pending;
                case WaterTransferRegistrationStatusEnum.Registered:
                    return Registered;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum WaterTransferRegistrationStatusEnum
    {
        Pending = 1,
        Registered = 2,
        Canceled = 3
    }

    public partial class WaterTransferRegistrationStatusPending : WaterTransferRegistrationStatus
    {
        private WaterTransferRegistrationStatusPending(int waterTransferRegistrationStatusID, string waterTransferRegistrationStatusName, string waterTransferRegistrationStatusDisplayName) : base(waterTransferRegistrationStatusID, waterTransferRegistrationStatusName, waterTransferRegistrationStatusDisplayName) {}
        public static readonly WaterTransferRegistrationStatusPending Instance = new WaterTransferRegistrationStatusPending(1, @"Pending", @"Pending");
    }

    public partial class WaterTransferRegistrationStatusRegistered : WaterTransferRegistrationStatus
    {
        private WaterTransferRegistrationStatusRegistered(int waterTransferRegistrationStatusID, string waterTransferRegistrationStatusName, string waterTransferRegistrationStatusDisplayName) : base(waterTransferRegistrationStatusID, waterTransferRegistrationStatusName, waterTransferRegistrationStatusDisplayName) {}
        public static readonly WaterTransferRegistrationStatusRegistered Instance = new WaterTransferRegistrationStatusRegistered(2, @"Registered", @"Registered");
    }

    public partial class WaterTransferRegistrationStatusCanceled : WaterTransferRegistrationStatus
    {
        private WaterTransferRegistrationStatusCanceled(int waterTransferRegistrationStatusID, string waterTransferRegistrationStatusName, string waterTransferRegistrationStatusDisplayName) : base(waterTransferRegistrationStatusID, waterTransferRegistrationStatusName, waterTransferRegistrationStatusDisplayName) {}
        public static readonly WaterTransferRegistrationStatusCanceled Instance = new WaterTransferRegistrationStatusCanceled(3, @"Canceled", @"Canceled");
    }
}