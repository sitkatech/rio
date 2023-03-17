using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("Account")]
    [Index("AccountNumber", Name = "AK_Account_AccountNumber", IsUnique = true)]
    public partial class Account
    {
        public Account()
        {
            AccountOverconsumptionCharges = new HashSet<AccountOverconsumptionCharge>();
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
        [Unicode(false)]
        public string AccountName { get; set; }
        public int AccountStatusID { get; set; }
        [Unicode(false)]
        public string Notes { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [StringLength(6)]
        [Unicode(false)]
        public string AccountVerificationKey { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AccountVerificationKeyLastUseDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InactivateDate { get; set; }

        [InverseProperty("Account")]
        public virtual ICollection<AccountOverconsumptionCharge> AccountOverconsumptionCharges { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<AccountParcelWaterYear> AccountParcelWaterYears { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<AccountReconciliation> AccountReconciliations { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<AccountUser> AccountUsers { get; set; }
        [InverseProperty("CreateAccount")]
        public virtual ICollection<Offer> Offers { get; set; }
        [InverseProperty("CreateAccount")]
        public virtual ICollection<Posting> Postings { get; set; }
        [InverseProperty("CreateAccount")]
        public virtual ICollection<Trade> Trades { get; set; }
        [InverseProperty("Account")]
        public virtual ICollection<WaterTransferRegistration> WaterTransferRegistrations { get; set; }
    }
}
