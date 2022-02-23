﻿using System;
using System.Collections.Generic;
using Rio.Models.DataTransferObjects.Account;

namespace Rio.Models.DataTransferObjects.User
{
    public class UserDetailedDto
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public Guid? UserGuid { get; set; }
        public string Phone { get; set; }
        public string LoginName { get; set; }
        public bool ReceiveSupportEmails { get; set; }
        public int RoleID { get; set; }
        public string RoleDisplayName { get; set; }

        public bool? HasActiveTrades { get; set; }
        public int? AcreFeetOfWaterPurchased { get; set; }
        public int? AcreFeetOfWaterSold { get; set; }
        public bool IsActive { get; set; }

        public List<AccountSimpleDto> AssociatedAccounts { get; set; }
    }
}