//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PostingStatus]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class PostingStatus
    {
        public static readonly PostingStatusOpen Open = Rio.EFModels.Entities.PostingStatusOpen.Instance;
        public static readonly PostingStatusClosed Closed = Rio.EFModels.Entities.PostingStatusClosed.Instance;

        public static readonly List<PostingStatus> All;
        public static readonly List<PostingStatusDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, PostingStatus> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, PostingStatusDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static PostingStatus()
        {
            All = new List<PostingStatus> { Open, Closed };
            AllAsDto = new List<PostingStatusDto> { Open.AsDto(), Closed.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, PostingStatus>(All.ToDictionary(x => x.PostingStatusID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, PostingStatusDto>(AllAsDto.ToDictionary(x => x.PostingStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected PostingStatus(int postingStatusID, string postingStatusName, string postingStatusDisplayName)
        {
            PostingStatusID = postingStatusID;
            PostingStatusName = postingStatusName;
            PostingStatusDisplayName = postingStatusDisplayName;
        }

        [Key]
        public int PostingStatusID { get; private set; }
        public string PostingStatusName { get; private set; }
        public string PostingStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return PostingStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(PostingStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.PostingStatusID == PostingStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as PostingStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return PostingStatusID;
        }

        public static bool operator ==(PostingStatus left, PostingStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PostingStatus left, PostingStatus right)
        {
            return !Equals(left, right);
        }

        public PostingStatusEnum ToEnum => (PostingStatusEnum)GetHashCode();

        public static PostingStatus ToType(int enumValue)
        {
            return ToType((PostingStatusEnum)enumValue);
        }

        public static PostingStatus ToType(PostingStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case PostingStatusEnum.Closed:
                    return Closed;
                case PostingStatusEnum.Open:
                    return Open;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum PostingStatusEnum
    {
        Open = 1,
        Closed = 2
    }

    public partial class PostingStatusOpen : PostingStatus
    {
        private PostingStatusOpen(int postingStatusID, string postingStatusName, string postingStatusDisplayName) : base(postingStatusID, postingStatusName, postingStatusDisplayName) {}
        public static readonly PostingStatusOpen Instance = new PostingStatusOpen(1, @"Open", @"Open");
    }

    public partial class PostingStatusClosed : PostingStatus
    {
        private PostingStatusClosed(int postingStatusID, string postingStatusName, string postingStatusDisplayName) : base(postingStatusID, postingStatusName, postingStatusDisplayName) {}
        public static readonly PostingStatusClosed Instance = new PostingStatusClosed(2, @"Closed", @"Closed");
    }
}