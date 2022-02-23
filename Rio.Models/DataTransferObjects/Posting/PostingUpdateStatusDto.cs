using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.Posting
{
    public class PostingUpdateStatusDto
    {
        [Required]
        public int PostingStatusID { get; set; }
    }
}