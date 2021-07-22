using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("Account")]
    [Index(nameof(AccountNumber), Name = "AK_Account_AccountNumber", IsUnique = true)]
    public partial class Account
    {
        public Account()
        {
            AccountParcelWaterYears = new HashSet<AccountParcelWaterYear>();
            AccountReconciliations = new HashSet<AccountReconciliation>();
            AccountUsers = new HashSet<AccountUser>();
            Offers = new HashSet<Offer>();
            Postings = new HashSet<Posting>();
            Trades = new HashSet<Trade>();
            WaterTransferRegistrations = new HashSet<WaterTransferRegistration>();
        }

        [Key]
        public int AccountID { get; set; }
        public int AccountNumber { get; set; }
        [StringLength(255)]
        public string AccountName { get; set; }
        public int AccountStatusID { get; set; }
        public string Notes { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [StringLength(6)]
        public string AccountVerificationKey { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AccountVerificationKeyLastUseDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InactivateDate { get; set; }

        [ForeignKey(nameof(AccountStatusID))]
        [InverseProperty("Accounts")]
        public virtual AccountStatus AccountStatus { get; set; }
        [InverseProperty(nameof(AccountParcelWaterYear.Account))]
        public virtual ICollection<AccountParcelWaterYear> AccountParcelWaterYears { get; set; }
        [InverseProperty(nameof(AccountReconciliation.Account))]
        public virtual ICollection<AccountReconciliation> AccountReconciliations { get; set; }
        [InverseProperty(nameof(AccountUser.Account))]
        public virtual ICollection<AccountUser> AccountUsers { get; set; }
        [InverseProperty(nameof(Offer.CreateAccount))]
        public virtual ICollection<Offer> Offers { get; set; }
        [InverseProperty(nameof(Posting.CreateAccount))]
        public virtual ICollection<Posting> Postings { get; set; }
        [InverseProperty(nameof(Trade.CreateAccount))]
        public virtual ICollection<Trade> Trades { get; set; }
        [InverseProperty(nameof(WaterTransferRegistration.Account))]
        public virtual ICollection<WaterTransferRegistration> WaterTransferRegistrations { get; set; }
    }
}
