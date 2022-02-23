﻿using System;

namespace Rio.Models.DataTransferObjects
{
    public class TransactionHistoryDto
    {
        public DateTime TransactionDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string CreateUserFullName { get; set; }
        public string WaterTypeName { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string UploadedFileName { get; set; }
        public int AffectedParcelsCount { get; set; }
    } 
}