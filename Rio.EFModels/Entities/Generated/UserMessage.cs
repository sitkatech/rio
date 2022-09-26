using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("UserMessage")]
    public partial class UserMessage
    {
        [Key]
        public int UserMessageID { get; set; }
        public int CreateUserID { get; set; }
        public int RecipientUserID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Required]
        [StringLength(5000)]
        [Unicode(false)]
        public string Message { get; set; }

        [ForeignKey("CreateUserID")]
        [InverseProperty("UserMessageCreateUsers")]
        public virtual User CreateUser { get; set; }
        [ForeignKey("RecipientUserID")]
        [InverseProperty("UserMessageRecipientUsers")]
        public virtual User RecipientUser { get; set; }
    }
}
