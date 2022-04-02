//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PostingType]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class PostingType
    {
        public static readonly PostingTypeOfferToBuy OfferToBuy = Rio.EFModels.Entities.PostingTypeOfferToBuy.Instance;
        public static readonly PostingTypeOfferToSell OfferToSell = Rio.EFModels.Entities.PostingTypeOfferToSell.Instance;

        public static readonly List<PostingType> All;
        public static readonly List<PostingTypeDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, PostingType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, PostingTypeDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static PostingType()
        {
            All = new List<PostingType> { OfferToBuy, OfferToSell };
            AllAsDto = new List<PostingTypeDto> { OfferToBuy.AsDto(), OfferToSell.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, PostingType>(All.ToDictionary(x => x.PostingTypeID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, PostingTypeDto>(AllAsDto.ToDictionary(x => x.PostingTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected PostingType(int postingTypeID, string postingTypeName, string postingTypeDisplayName)
        {
            PostingTypeID = postingTypeID;
            PostingTypeName = postingTypeName;
            PostingTypeDisplayName = postingTypeDisplayName;
        }

        [Key]
        public int PostingTypeID { get; private set; }
        public string PostingTypeName { get; private set; }
        public string PostingTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return PostingTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(PostingType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.PostingTypeID == PostingTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as PostingType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return PostingTypeID;
        }

        public static bool operator ==(PostingType left, PostingType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PostingType left, PostingType right)
        {
            return !Equals(left, right);
        }

        public PostingTypeEnum ToEnum => (PostingTypeEnum)GetHashCode();

        public static PostingType ToType(int enumValue)
        {
            return ToType((PostingTypeEnum)enumValue);
        }

        public static PostingType ToType(PostingTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case PostingTypeEnum.OfferToBuy:
                    return OfferToBuy;
                case PostingTypeEnum.OfferToSell:
                    return OfferToSell;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum PostingTypeEnum
    {
        OfferToBuy = 1,
        OfferToSell = 2
    }

    public partial class PostingTypeOfferToBuy : PostingType
    {
        private PostingTypeOfferToBuy(int postingTypeID, string postingTypeName, string postingTypeDisplayName) : base(postingTypeID, postingTypeName, postingTypeDisplayName) {}
        public static readonly PostingTypeOfferToBuy Instance = new PostingTypeOfferToBuy(1, @"OfferToBuy", @"Offer to Buy");
    }

    public partial class PostingTypeOfferToSell : PostingType
    {
        private PostingTypeOfferToSell(int postingTypeID, string postingTypeName, string postingTypeDisplayName) : base(postingTypeID, postingTypeName, postingTypeDisplayName) {}
        public static readonly PostingTypeOfferToSell Instance = new PostingTypeOfferToSell(2, @"OfferToSell", @"Offer to Sell");
    }
}