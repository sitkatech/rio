//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Role]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class Role
    {
        public static readonly RoleAdmin Admin = Rio.EFModels.Entities.RoleAdmin.Instance;
        public static readonly RoleLandOwner LandOwner = Rio.EFModels.Entities.RoleLandOwner.Instance;
        public static readonly RoleUnassigned Unassigned = Rio.EFModels.Entities.RoleUnassigned.Instance;
        public static readonly RoleDisabled Disabled = Rio.EFModels.Entities.RoleDisabled.Instance;
        public static readonly RoleDemoUser DemoUser = Rio.EFModels.Entities.RoleDemoUser.Instance;

        public static readonly List<Role> All;
        public static readonly List<RoleDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, Role> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, RoleDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static Role()
        {
            All = new List<Role> { Admin, LandOwner, Unassigned, Disabled, DemoUser };
            AllAsDto = new List<RoleDto> { Admin.AsDto(), LandOwner.AsDto(), Unassigned.AsDto(), Disabled.AsDto(), DemoUser.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, Role>(All.ToDictionary(x => x.RoleID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, RoleDto>(AllAsDto.ToDictionary(x => x.RoleID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected Role(int roleID, string roleName, string roleDisplayName, string roleDescription, int sortOrder)
        {
            RoleID = roleID;
            RoleName = roleName;
            RoleDisplayName = roleDisplayName;
            RoleDescription = roleDescription;
            SortOrder = sortOrder;
        }

        [Key]
        public int RoleID { get; private set; }
        public string RoleName { get; private set; }
        public string RoleDisplayName { get; private set; }
        public string RoleDescription { get; private set; }
        public int SortOrder { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return RoleID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(Role other)
        {
            if (other == null)
            {
                return false;
            }
            return other.RoleID == RoleID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as Role);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return RoleID;
        }

        public static bool operator ==(Role left, Role right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Role left, Role right)
        {
            return !Equals(left, right);
        }

        public RoleEnum ToEnum => (RoleEnum)GetHashCode();

        public static Role ToType(int enumValue)
        {
            return ToType((RoleEnum)enumValue);
        }

        public static Role ToType(RoleEnum enumValue)
        {
            switch (enumValue)
            {
                case RoleEnum.Admin:
                    return Admin;
                case RoleEnum.DemoUser:
                    return DemoUser;
                case RoleEnum.Disabled:
                    return Disabled;
                case RoleEnum.LandOwner:
                    return LandOwner;
                case RoleEnum.Unassigned:
                    return Unassigned;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum RoleEnum
    {
        Admin = 1,
        LandOwner = 2,
        Unassigned = 3,
        Disabled = 4,
        DemoUser = 5
    }

    public partial class RoleAdmin : Role
    {
        private RoleAdmin(int roleID, string roleName, string roleDisplayName, string roleDescription, int sortOrder) : base(roleID, roleName, roleDisplayName, roleDescription, sortOrder) {}
        public static readonly RoleAdmin Instance = new RoleAdmin(1, @"Admin", @"Administrator", @"", 30);
    }

    public partial class RoleLandOwner : Role
    {
        private RoleLandOwner(int roleID, string roleName, string roleDisplayName, string roleDescription, int sortOrder) : base(roleID, roleName, roleDisplayName, roleDescription, sortOrder) {}
        public static readonly RoleLandOwner Instance = new RoleLandOwner(2, @"LandOwner", @"Landowner", @"", 20);
    }

    public partial class RoleUnassigned : Role
    {
        private RoleUnassigned(int roleID, string roleName, string roleDisplayName, string roleDescription, int sortOrder) : base(roleID, roleName, roleDisplayName, roleDescription, sortOrder) {}
        public static readonly RoleUnassigned Instance = new RoleUnassigned(3, @"Unassigned", @"Unassigned", @"", 10);
    }

    public partial class RoleDisabled : Role
    {
        private RoleDisabled(int roleID, string roleName, string roleDisplayName, string roleDescription, int sortOrder) : base(roleID, roleName, roleDisplayName, roleDescription, sortOrder) {}
        public static readonly RoleDisabled Instance = new RoleDisabled(4, @"Disabled", @"Disabled", @"", 40);
    }

    public partial class RoleDemoUser : Role
    {
        private RoleDemoUser(int roleID, string roleName, string roleDisplayName, string roleDescription, int sortOrder) : base(roleID, roleName, roleDisplayName, roleDescription, sortOrder) {}
        public static readonly RoleDemoUser Instance = new RoleDemoUser(5, @"DemoUser", @"Demo User", @"", 35);
    }
}