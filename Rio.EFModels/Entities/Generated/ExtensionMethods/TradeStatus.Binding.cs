//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TradeStatus]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class TradeStatus
    {
        public static readonly TradeStatusCountered Countered = Rio.EFModels.Entities.TradeStatusCountered.Instance;
        public static readonly TradeStatusAccepted Accepted = Rio.EFModels.Entities.TradeStatusAccepted.Instance;
        public static readonly TradeStatusRejected Rejected = Rio.EFModels.Entities.TradeStatusRejected.Instance;
        public static readonly TradeStatusRescinded Rescinded = Rio.EFModels.Entities.TradeStatusRescinded.Instance;

        public static readonly List<TradeStatus> All;
        public static readonly List<TradeStatusDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, TradeStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, TradeStatusDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static TradeStatus()
        {
            All = new List<TradeStatus> { Countered, Accepted, Rejected, Rescinded };
            AllAsDto = new List<TradeStatusDto> { Countered.AsDto(), Accepted.AsDto(), Rejected.AsDto(), Rescinded.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, TradeStatus>(All.ToDictionary(x => x.TradeStatusID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, TradeStatusDto>(AllAsDto.ToDictionary(x => x.TradeStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected TradeStatus(int tradeStatusID, string tradeStatusName, string tradeStatusDisplayName)
        {
            TradeStatusID = tradeStatusID;
            TradeStatusName = tradeStatusName;
            TradeStatusDisplayName = tradeStatusDisplayName;
        }

        [Key]
        public int TradeStatusID { get; private set; }
        public string TradeStatusName { get; private set; }
        public string TradeStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return TradeStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(TradeStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.TradeStatusID == TradeStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as TradeStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return TradeStatusID;
        }

        public static bool operator ==(TradeStatus left, TradeStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TradeStatus left, TradeStatus right)
        {
            return !Equals(left, right);
        }

        public TradeStatusEnum ToEnum => (TradeStatusEnum)GetHashCode();

        public static TradeStatus ToType(int enumValue)
        {
            return ToType((TradeStatusEnum)enumValue);
        }

        public static TradeStatus ToType(TradeStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case TradeStatusEnum.Accepted:
                    return Accepted;
                case TradeStatusEnum.Countered:
                    return Countered;
                case TradeStatusEnum.Rejected:
                    return Rejected;
                case TradeStatusEnum.Rescinded:
                    return Rescinded;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum TradeStatusEnum
    {
        Countered = 1,
        Accepted = 2,
        Rejected = 3,
        Rescinded = 4
    }

    public partial class TradeStatusCountered : TradeStatus
    {
        private TradeStatusCountered(int tradeStatusID, string tradeStatusName, string tradeStatusDisplayName) : base(tradeStatusID, tradeStatusName, tradeStatusDisplayName) {}
        public static readonly TradeStatusCountered Instance = new TradeStatusCountered(1, @"Countered", @"Countered");
    }

    public partial class TradeStatusAccepted : TradeStatus
    {
        private TradeStatusAccepted(int tradeStatusID, string tradeStatusName, string tradeStatusDisplayName) : base(tradeStatusID, tradeStatusName, tradeStatusDisplayName) {}
        public static readonly TradeStatusAccepted Instance = new TradeStatusAccepted(2, @"Accepted", @"Accepted");
    }

    public partial class TradeStatusRejected : TradeStatus
    {
        private TradeStatusRejected(int tradeStatusID, string tradeStatusName, string tradeStatusDisplayName) : base(tradeStatusID, tradeStatusName, tradeStatusDisplayName) {}
        public static readonly TradeStatusRejected Instance = new TradeStatusRejected(3, @"Rejected", @"Rejected");
    }

    public partial class TradeStatusRescinded : TradeStatus
    {
        private TradeStatusRescinded(int tradeStatusID, string tradeStatusName, string tradeStatusDisplayName) : base(tradeStatusID, tradeStatusName, tradeStatusDisplayName) {}
        public static readonly TradeStatusRescinded Instance = new TradeStatusRescinded(4, @"Rescinded", @"Rescinded");
    }
}