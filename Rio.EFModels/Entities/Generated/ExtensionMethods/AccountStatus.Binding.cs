//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AccountStatus]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class AccountStatus
    {
        public static readonly AccountStatusActive Active = Rio.EFModels.Entities.AccountStatusActive.Instance;
        public static readonly AccountStatusInactive Inactive = Rio.EFModels.Entities.AccountStatusInactive.Instance;

        public static readonly List<AccountStatus> All;
        public static readonly List<AccountStatusDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, AccountStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, AccountStatusDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static AccountStatus()
        {
            All = new List<AccountStatus> { Active, Inactive };
            AllAsDto = new List<AccountStatusDto> { Active.AsDto(), Inactive.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, AccountStatus>(All.ToDictionary(x => x.AccountStatusID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, AccountStatusDto>(AllAsDto.ToDictionary(x => x.AccountStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected AccountStatus(int accountStatusID, string accountStatusName, string accountStatusDisplayName)
        {
            AccountStatusID = accountStatusID;
            AccountStatusName = accountStatusName;
            AccountStatusDisplayName = accountStatusDisplayName;
        }

        [Key]
        public int AccountStatusID { get; private set; }
        public string AccountStatusName { get; private set; }
        public string AccountStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return AccountStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(AccountStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.AccountStatusID == AccountStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as AccountStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return AccountStatusID;
        }

        public static bool operator ==(AccountStatus left, AccountStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AccountStatus left, AccountStatus right)
        {
            return !Equals(left, right);
        }

        public AccountStatusEnum ToEnum => (AccountStatusEnum)GetHashCode();

        public static AccountStatus ToType(int enumValue)
        {
            return ToType((AccountStatusEnum)enumValue);
        }

        public static AccountStatus ToType(AccountStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case AccountStatusEnum.Active:
                    return Active;
                case AccountStatusEnum.Inactive:
                    return Inactive;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum AccountStatusEnum
    {
        Active = 1,
        Inactive = 2
    }

    public partial class AccountStatusActive : AccountStatus
    {
        private AccountStatusActive(int accountStatusID, string accountStatusName, string accountStatusDisplayName) : base(accountStatusID, accountStatusName, accountStatusDisplayName) {}
        public static readonly AccountStatusActive Instance = new AccountStatusActive(1, @"Active", @"Active");
    }

    public partial class AccountStatusInactive : AccountStatus
    {
        private AccountStatusInactive(int accountStatusID, string accountStatusName, string accountStatusDisplayName) : base(accountStatusID, accountStatusName, accountStatusDisplayName) {}
        public static readonly AccountStatusInactive Instance = new AccountStatusInactive(2, @"Inactive", @"Inactive");
    }
}