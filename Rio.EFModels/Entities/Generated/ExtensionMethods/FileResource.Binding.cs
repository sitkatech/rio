//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FileResource]
namespace Rio.EFModels.Entities
{
    public partial class FileResource
    {
        public FileResourceMimeType FileResourceMimeType => FileResourceMimeType.AllLookupDictionary[FileResourceMimeTypeID];
    }
}