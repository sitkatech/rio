//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETSyncResultType]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class OpenETSyncResultTypeExtensionMethods
    {
        public static OpenETSyncResultTypeDto AsDto(this OpenETSyncResultType openETSyncResultType)
        {
            var openETSyncResultTypeDto = new OpenETSyncResultTypeDto()
            {
                OpenETSyncResultTypeID = openETSyncResultType.OpenETSyncResultTypeID,
                OpenETSyncResultTypeName = openETSyncResultType.OpenETSyncResultTypeName,
                OpenETSyncResultTypeDisplayName = openETSyncResultType.OpenETSyncResultTypeDisplayName
            };
            DoCustomMappings(openETSyncResultType, openETSyncResultTypeDto);
            return openETSyncResultTypeDto;
        }

        static partial void DoCustomMappings(OpenETSyncResultType openETSyncResultType, OpenETSyncResultTypeDto openETSyncResultTypeDto);

        public static OpenETSyncResultTypeSimpleDto AsSimpleDto(this OpenETSyncResultType openETSyncResultType)
        {
            var openETSyncResultTypeSimpleDto = new OpenETSyncResultTypeSimpleDto()
            {
                OpenETSyncResultTypeID = openETSyncResultType.OpenETSyncResultTypeID,
                OpenETSyncResultTypeName = openETSyncResultType.OpenETSyncResultTypeName,
                OpenETSyncResultTypeDisplayName = openETSyncResultType.OpenETSyncResultTypeDisplayName
            };
            DoCustomSimpleDtoMappings(openETSyncResultType, openETSyncResultTypeSimpleDto);
            return openETSyncResultTypeSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(OpenETSyncResultType openETSyncResultType, OpenETSyncResultTypeSimpleDto openETSyncResultTypeSimpleDto);
    }
}