//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TransactionType]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class TransactionTypeExtensionMethods
    {
        public static TransactionTypeDto AsDto(this TransactionType transactionType)
        {
            var transactionTypeDto = new TransactionTypeDto()
            {
                TransactionTypeID = transactionType.TransactionTypeID,
                TransactionTypeName = transactionType.TransactionTypeName
            };
            DoCustomMappings(transactionType, transactionTypeDto);
            return transactionTypeDto;
        }

        static partial void DoCustomMappings(TransactionType transactionType, TransactionTypeDto transactionTypeDto);

        public static TransactionTypeSimpleDto AsSimpleDto(this TransactionType transactionType)
        {
            var transactionTypeSimpleDto = new TransactionTypeSimpleDto()
            {
                TransactionTypeID = transactionType.TransactionTypeID,
                TransactionTypeName = transactionType.TransactionTypeName
            };
            DoCustomSimpleDtoMappings(transactionType, transactionTypeSimpleDto);
            return transactionTypeSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(TransactionType transactionType, TransactionTypeSimpleDto transactionTypeSimpleDto);
    }
}