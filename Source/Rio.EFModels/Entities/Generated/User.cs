﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class User
    {
        public User()
        {
            FileResource = new HashSet<FileResource>();
            Offer = new HashSet<Offer>();
            Posting = new HashSet<Posting>();
            Trade = new HashSet<Trade>();
            UserParcel = new HashSet<UserParcel>();
            WaterTransferReceivingUser = new HashSet<WaterTransfer>();
            WaterTransferTransferringUser = new HashSet<WaterTransfer>();
        }

        public int UserID { get; set; }
        public Guid? UserGuid { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [StringLength(255)]
        public string Email { get; set; }
        [StringLength(30)]
        public string Phone { get; set; }
        public int RoleID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastActivityDate { get; set; }
        public bool IsActive { get; set; }
        public bool ReceiveSupportEmails { get; set; }
        [StringLength(128)]
        public string LoginName { get; set; }
        [StringLength(100)]
        public string Company { get; set; }

        [ForeignKey("RoleID")]
        [InverseProperty("User")]
        public virtual Role Role { get; set; }
        [InverseProperty("CreateUser")]
        public virtual ICollection<FileResource> FileResource { get; set; }
        [InverseProperty("CreateUser")]
        public virtual ICollection<Offer> Offer { get; set; }
        [InverseProperty("CreateUser")]
        public virtual ICollection<Posting> Posting { get; set; }
        [InverseProperty("CreateUser")]
        public virtual ICollection<Trade> Trade { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserParcel> UserParcel { get; set; }
        [InverseProperty("ReceivingUser")]
        public virtual ICollection<WaterTransfer> WaterTransferReceivingUser { get; set; }
        [InverseProperty("TransferringUser")]
        public virtual ICollection<WaterTransfer> WaterTransferTransferringUser { get; set; }
    }
}
