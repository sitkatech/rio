//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TransactionType]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class TransactionType
    {
        public static readonly TransactionTypeSupply Supply = Rio.EFModels.Entities.TransactionTypeSupply.Instance;
        public static readonly TransactionTypeUsage Usage = Rio.EFModels.Entities.TransactionTypeUsage.Instance;

        public static readonly List<TransactionType> All;
        public static readonly List<TransactionTypeDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, TransactionType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, TransactionTypeDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static TransactionType()
        {
            All = new List<TransactionType> { Supply, Usage };
            AllAsDto = new List<TransactionTypeDto> { Supply.AsDto(), Usage.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, TransactionType>(All.ToDictionary(x => x.TransactionTypeID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, TransactionTypeDto>(AllAsDto.ToDictionary(x => x.TransactionTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected TransactionType(int transactionTypeID, string transactionTypeName, string transactionTypeDisplayName)
        {
            TransactionTypeID = transactionTypeID;
            TransactionTypeName = transactionTypeName;
            TransactionTypeDisplayName = transactionTypeDisplayName;
        }

        [Key]
        public int TransactionTypeID { get; private set; }
        public string TransactionTypeName { get; private set; }
        public string TransactionTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return TransactionTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(TransactionType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.TransactionTypeID == TransactionTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as TransactionType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return TransactionTypeID;
        }

        public static bool operator ==(TransactionType left, TransactionType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TransactionType left, TransactionType right)
        {
            return !Equals(left, right);
        }

        public TransactionTypeEnum ToEnum => (TransactionTypeEnum)GetHashCode();

        public static TransactionType ToType(int enumValue)
        {
            return ToType((TransactionTypeEnum)enumValue);
        }

        public static TransactionType ToType(TransactionTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case TransactionTypeEnum.Supply:
                    return Supply;
                case TransactionTypeEnum.Usage:
                    return Usage;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum TransactionTypeEnum
    {
        Supply = 1,
        Usage = 2
    }

    public partial class TransactionTypeSupply : TransactionType
    {
        private TransactionTypeSupply(int transactionTypeID, string transactionTypeName, string transactionTypeDisplayName) : base(transactionTypeID, transactionTypeName, transactionTypeDisplayName) {}
        public static readonly TransactionTypeSupply Instance = new TransactionTypeSupply(1, @"Supply", @"Supply");
    }

    public partial class TransactionTypeUsage : TransactionType
    {
        private TransactionTypeUsage(int transactionTypeID, string transactionTypeName, string transactionTypeDisplayName) : base(transactionTypeID, transactionTypeName, transactionTypeDisplayName) {}
        public static readonly TransactionTypeUsage Instance = new TransactionTypeUsage(2, @"Usage", @"Usage");
    }
}