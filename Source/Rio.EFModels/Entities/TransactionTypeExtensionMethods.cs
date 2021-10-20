using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class TransactionTypeExtensionMethods
    {
        public static TransactionTypeDto AsDto(this TransactionType transactionType)
        {
            return new TransactionTypeDto()
            {
                TransactionTypeID = transactionType.TransactionTypeID,
                TransactionTypeName = transactionType.TransactionTypeName,
                SortOrder = transactionType.SortOrder
            };

        }
    }
}
