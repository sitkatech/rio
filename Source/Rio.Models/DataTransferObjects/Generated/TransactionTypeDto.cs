//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TransactionType]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class TransactionTypeDto
    {
        public int TransactionTypeID { get; set; }
        public string TransactionTypeName { get; set; }
        public int SortOrder { get; set; }
    }

    public partial class TransactionTypeSimpleDto
    {
        public int TransactionTypeID { get; set; }
        public string TransactionTypeName { get; set; }
        public int SortOrder { get; set; }
    }

}