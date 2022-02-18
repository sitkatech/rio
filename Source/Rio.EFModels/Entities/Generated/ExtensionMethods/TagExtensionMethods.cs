//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Tag]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class TagExtensionMethods
    {
        public static TagDto AsDto(this Tag tag)
        {
            var tagDto = new TagDto()
            {
                TagID = tag.TagID,
                TagName = tag.TagName,
                TagDescription = tag.TagDescription
            };
            DoCustomMappings(tag, tagDto);
            return tagDto;
        }

        static partial void DoCustomMappings(Tag tag, TagDto tagDto);

        public static TagSimpleDto AsSimpleDto(this Tag tag)
        {
            var tagSimpleDto = new TagSimpleDto()
            {
                TagID = tag.TagID,
                TagName = tag.TagName,
                TagDescription = tag.TagDescription
            };
            DoCustomSimpleDtoMappings(tag, tagSimpleDto);
            return tagSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(Tag tag, TagSimpleDto tagSimpleDto);
    }
}