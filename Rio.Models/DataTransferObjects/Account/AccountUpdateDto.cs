namespace Rio.Models.DataTransferObjects.Account
{
    public class AccountUpdateDto
    {
        public string AccountName { get; set; }
        public string Notes { get; set; }
        public int AccountStatusID { get; set; }
    }
}