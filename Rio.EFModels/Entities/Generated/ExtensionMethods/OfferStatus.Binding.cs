//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OfferStatus]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class OfferStatus
    {
        public static readonly OfferStatusPending Pending = Rio.EFModels.Entities.OfferStatusPending.Instance;
        public static readonly OfferStatusRejected Rejected = Rio.EFModels.Entities.OfferStatusRejected.Instance;
        public static readonly OfferStatusRescinded Rescinded = Rio.EFModels.Entities.OfferStatusRescinded.Instance;
        public static readonly OfferStatusAccepted Accepted = Rio.EFModels.Entities.OfferStatusAccepted.Instance;

        public static readonly List<OfferStatus> All;
        public static readonly List<OfferStatusDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, OfferStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, OfferStatusDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static OfferStatus()
        {
            All = new List<OfferStatus> { Pending, Rejected, Rescinded, Accepted };
            AllAsDto = new List<OfferStatusDto> { Pending.AsDto(), Rejected.AsDto(), Rescinded.AsDto(), Accepted.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, OfferStatus>(All.ToDictionary(x => x.OfferStatusID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, OfferStatusDto>(AllAsDto.ToDictionary(x => x.OfferStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected OfferStatus(int offerStatusID, string offerStatusName, string offerStatusDisplayName)
        {
            OfferStatusID = offerStatusID;
            OfferStatusName = offerStatusName;
            OfferStatusDisplayName = offerStatusDisplayName;
        }

        [Key]
        public int OfferStatusID { get; private set; }
        public string OfferStatusName { get; private set; }
        public string OfferStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return OfferStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(OfferStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.OfferStatusID == OfferStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as OfferStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return OfferStatusID;
        }

        public static bool operator ==(OfferStatus left, OfferStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OfferStatus left, OfferStatus right)
        {
            return !Equals(left, right);
        }

        public OfferStatusEnum ToEnum => (OfferStatusEnum)GetHashCode();

        public static OfferStatus ToType(int enumValue)
        {
            return ToType((OfferStatusEnum)enumValue);
        }

        public static OfferStatus ToType(OfferStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case OfferStatusEnum.Accepted:
                    return Accepted;
                case OfferStatusEnum.Pending:
                    return Pending;
                case OfferStatusEnum.Rejected:
                    return Rejected;
                case OfferStatusEnum.Rescinded:
                    return Rescinded;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum OfferStatusEnum
    {
        Pending = 1,
        Rejected = 2,
        Rescinded = 3,
        Accepted = 4
    }

    public partial class OfferStatusPending : OfferStatus
    {
        private OfferStatusPending(int offerStatusID, string offerStatusName, string offerStatusDisplayName) : base(offerStatusID, offerStatusName, offerStatusDisplayName) {}
        public static readonly OfferStatusPending Instance = new OfferStatusPending(1, @"Pending", @"Pending");
    }

    public partial class OfferStatusRejected : OfferStatus
    {
        private OfferStatusRejected(int offerStatusID, string offerStatusName, string offerStatusDisplayName) : base(offerStatusID, offerStatusName, offerStatusDisplayName) {}
        public static readonly OfferStatusRejected Instance = new OfferStatusRejected(2, @"Rejected", @"Rejected");
    }

    public partial class OfferStatusRescinded : OfferStatus
    {
        private OfferStatusRescinded(int offerStatusID, string offerStatusName, string offerStatusDisplayName) : base(offerStatusID, offerStatusName, offerStatusDisplayName) {}
        public static readonly OfferStatusRescinded Instance = new OfferStatusRescinded(3, @"Rescinded", @"Rescinded");
    }

    public partial class OfferStatusAccepted : OfferStatus
    {
        private OfferStatusAccepted(int offerStatusID, string offerStatusName, string offerStatusDisplayName) : base(offerStatusID, offerStatusName, offerStatusDisplayName) {}
        public static readonly OfferStatusAccepted Instance = new OfferStatusAccepted(4, @"Accepted", @"Accepted");
    }
}