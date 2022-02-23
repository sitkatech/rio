//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UploadedGdb]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class UploadedGdbExtensionMethods
    {
        public static UploadedGdbDto AsDto(this UploadedGdb uploadedGdb)
        {
            var uploadedGdbDto = new UploadedGdbDto()
            {
                UploadedGdbID = uploadedGdb.UploadedGdbID,
                GdbFileContents = uploadedGdb.GdbFileContents,
                UploadDate = uploadedGdb.UploadDate
            };
            DoCustomMappings(uploadedGdb, uploadedGdbDto);
            return uploadedGdbDto;
        }

        static partial void DoCustomMappings(UploadedGdb uploadedGdb, UploadedGdbDto uploadedGdbDto);

        public static UploadedGdbSimpleDto AsSimpleDto(this UploadedGdb uploadedGdb)
        {
            var uploadedGdbSimpleDto = new UploadedGdbSimpleDto()
            {
                UploadedGdbID = uploadedGdb.UploadedGdbID,
                GdbFileContents = uploadedGdb.GdbFileContents,
                UploadDate = uploadedGdb.UploadDate
            };
            DoCustomSimpleDtoMappings(uploadedGdb, uploadedGdbSimpleDto);
            return uploadedGdbSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(UploadedGdb uploadedGdb, UploadedGdbSimpleDto uploadedGdbSimpleDto);
    }
}