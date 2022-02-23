//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FileResource]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class FileResourceExtensionMethods
    {
        public static FileResourceDto AsDto(this FileResource fileResource)
        {
            var fileResourceDto = new FileResourceDto()
            {
                FileResourceID = fileResource.FileResourceID,
                FileResourceMimeType = fileResource.FileResourceMimeType.AsDto(),
                OriginalBaseFilename = fileResource.OriginalBaseFilename,
                OriginalFileExtension = fileResource.OriginalFileExtension,
                FileResourceGUID = fileResource.FileResourceGUID,
                FileResourceData = fileResource.FileResourceData,
                CreateUser = fileResource.CreateUser.AsDto(),
                CreateDate = fileResource.CreateDate
            };
            DoCustomMappings(fileResource, fileResourceDto);
            return fileResourceDto;
        }

        static partial void DoCustomMappings(FileResource fileResource, FileResourceDto fileResourceDto);

        public static FileResourceSimpleDto AsSimpleDto(this FileResource fileResource)
        {
            var fileResourceSimpleDto = new FileResourceSimpleDto()
            {
                FileResourceID = fileResource.FileResourceID,
                FileResourceMimeTypeID = fileResource.FileResourceMimeTypeID,
                OriginalBaseFilename = fileResource.OriginalBaseFilename,
                OriginalFileExtension = fileResource.OriginalFileExtension,
                FileResourceGUID = fileResource.FileResourceGUID,
                FileResourceData = fileResource.FileResourceData,
                CreateUserID = fileResource.CreateUserID,
                CreateDate = fileResource.CreateDate
            };
            DoCustomSimpleDtoMappings(fileResource, fileResourceSimpleDto);
            return fileResourceSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(FileResource fileResource, FileResourceSimpleDto fileResourceSimpleDto);
    }
}