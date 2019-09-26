using System;
using System.ComponentModel.DataAnnotations;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.WaterTransfer
{
    public class WaterTransferRegistrationSimpleDto
    {
        [Required]
        public int WaterTransferTypeID { get; set; }
        [Required]
        public UserSimpleDto User { get; set; }
        public DateTime? DateRegistered { get; set; }
    }
}