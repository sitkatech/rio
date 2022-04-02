//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETSyncResultType]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class OpenETSyncResultType
    {
        public static readonly OpenETSyncResultTypeInProgress InProgress = Rio.EFModels.Entities.OpenETSyncResultTypeInProgress.Instance;
        public static readonly OpenETSyncResultTypeSucceeded Succeeded = Rio.EFModels.Entities.OpenETSyncResultTypeSucceeded.Instance;
        public static readonly OpenETSyncResultTypeFailed Failed = Rio.EFModels.Entities.OpenETSyncResultTypeFailed.Instance;
        public static readonly OpenETSyncResultTypeNoNewData NoNewData = Rio.EFModels.Entities.OpenETSyncResultTypeNoNewData.Instance;
        public static readonly OpenETSyncResultTypeDataNotAvailable DataNotAvailable = Rio.EFModels.Entities.OpenETSyncResultTypeDataNotAvailable.Instance;
        public static readonly OpenETSyncResultTypeCreated Created = Rio.EFModels.Entities.OpenETSyncResultTypeCreated.Instance;

        public static readonly List<OpenETSyncResultType> All;
        public static readonly List<OpenETSyncResultTypeDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, OpenETSyncResultType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, OpenETSyncResultTypeDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static OpenETSyncResultType()
        {
            All = new List<OpenETSyncResultType> { InProgress, Succeeded, Failed, NoNewData, DataNotAvailable, Created };
            AllAsDto = new List<OpenETSyncResultTypeDto> { InProgress.AsDto(), Succeeded.AsDto(), Failed.AsDto(), NoNewData.AsDto(), DataNotAvailable.AsDto(), Created.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, OpenETSyncResultType>(All.ToDictionary(x => x.OpenETSyncResultTypeID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, OpenETSyncResultTypeDto>(AllAsDto.ToDictionary(x => x.OpenETSyncResultTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected OpenETSyncResultType(int openETSyncResultTypeID, string openETSyncResultTypeName, string openETSyncResultTypeDisplayName)
        {
            OpenETSyncResultTypeID = openETSyncResultTypeID;
            OpenETSyncResultTypeName = openETSyncResultTypeName;
            OpenETSyncResultTypeDisplayName = openETSyncResultTypeDisplayName;
        }

        [Key]
        public int OpenETSyncResultTypeID { get; private set; }
        public string OpenETSyncResultTypeName { get; private set; }
        public string OpenETSyncResultTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return OpenETSyncResultTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(OpenETSyncResultType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.OpenETSyncResultTypeID == OpenETSyncResultTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as OpenETSyncResultType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return OpenETSyncResultTypeID;
        }

        public static bool operator ==(OpenETSyncResultType left, OpenETSyncResultType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OpenETSyncResultType left, OpenETSyncResultType right)
        {
            return !Equals(left, right);
        }

        public OpenETSyncResultTypeEnum ToEnum => (OpenETSyncResultTypeEnum)GetHashCode();

        public static OpenETSyncResultType ToType(int enumValue)
        {
            return ToType((OpenETSyncResultTypeEnum)enumValue);
        }

        public static OpenETSyncResultType ToType(OpenETSyncResultTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case OpenETSyncResultTypeEnum.Created:
                    return Created;
                case OpenETSyncResultTypeEnum.DataNotAvailable:
                    return DataNotAvailable;
                case OpenETSyncResultTypeEnum.Failed:
                    return Failed;
                case OpenETSyncResultTypeEnum.InProgress:
                    return InProgress;
                case OpenETSyncResultTypeEnum.NoNewData:
                    return NoNewData;
                case OpenETSyncResultTypeEnum.Succeeded:
                    return Succeeded;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum OpenETSyncResultTypeEnum
    {
        InProgress = 1,
        Succeeded = 2,
        Failed = 3,
        NoNewData = 4,
        DataNotAvailable = 5,
        Created = 6
    }

    public partial class OpenETSyncResultTypeInProgress : OpenETSyncResultType
    {
        private OpenETSyncResultTypeInProgress(int openETSyncResultTypeID, string openETSyncResultTypeName, string openETSyncResultTypeDisplayName) : base(openETSyncResultTypeID, openETSyncResultTypeName, openETSyncResultTypeDisplayName) {}
        public static readonly OpenETSyncResultTypeInProgress Instance = new OpenETSyncResultTypeInProgress(1, @"InProgress", @"In Progress");
    }

    public partial class OpenETSyncResultTypeSucceeded : OpenETSyncResultType
    {
        private OpenETSyncResultTypeSucceeded(int openETSyncResultTypeID, string openETSyncResultTypeName, string openETSyncResultTypeDisplayName) : base(openETSyncResultTypeID, openETSyncResultTypeName, openETSyncResultTypeDisplayName) {}
        public static readonly OpenETSyncResultTypeSucceeded Instance = new OpenETSyncResultTypeSucceeded(2, @"Succeeded", @"Succeeded");
    }

    public partial class OpenETSyncResultTypeFailed : OpenETSyncResultType
    {
        private OpenETSyncResultTypeFailed(int openETSyncResultTypeID, string openETSyncResultTypeName, string openETSyncResultTypeDisplayName) : base(openETSyncResultTypeID, openETSyncResultTypeName, openETSyncResultTypeDisplayName) {}
        public static readonly OpenETSyncResultTypeFailed Instance = new OpenETSyncResultTypeFailed(3, @"Failed", @"Failed");
    }

    public partial class OpenETSyncResultTypeNoNewData : OpenETSyncResultType
    {
        private OpenETSyncResultTypeNoNewData(int openETSyncResultTypeID, string openETSyncResultTypeName, string openETSyncResultTypeDisplayName) : base(openETSyncResultTypeID, openETSyncResultTypeName, openETSyncResultTypeDisplayName) {}
        public static readonly OpenETSyncResultTypeNoNewData Instance = new OpenETSyncResultTypeNoNewData(4, @"NoNewData", @"No New Data");
    }

    public partial class OpenETSyncResultTypeDataNotAvailable : OpenETSyncResultType
    {
        private OpenETSyncResultTypeDataNotAvailable(int openETSyncResultTypeID, string openETSyncResultTypeName, string openETSyncResultTypeDisplayName) : base(openETSyncResultTypeID, openETSyncResultTypeName, openETSyncResultTypeDisplayName) {}
        public static readonly OpenETSyncResultTypeDataNotAvailable Instance = new OpenETSyncResultTypeDataNotAvailable(5, @"DataNotAvailable", @"Data Not Available");
    }

    public partial class OpenETSyncResultTypeCreated : OpenETSyncResultType
    {
        private OpenETSyncResultTypeCreated(int openETSyncResultTypeID, string openETSyncResultTypeName, string openETSyncResultTypeDisplayName) : base(openETSyncResultTypeID, openETSyncResultTypeName, openETSyncResultTypeDisplayName) {}
        public static readonly OpenETSyncResultTypeCreated Instance = new OpenETSyncResultTypeCreated(6, @"Created", @"Created");
    }
}