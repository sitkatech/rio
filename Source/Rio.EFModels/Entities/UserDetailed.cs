using System;

namespace Rio.EFModels.Entities
{
    public partial class UserDetailed
    {
        public UserDetailed()
        {
        }

        public int UserID { get; set; }
        public Guid? UserGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string LoginName { get; set; }

        public string Phone { get; set; }
        public string Company { get; set; }
        public int RoleID { get; set; }
        public string RoleDisplayName { get; set; }

        public bool HasActiveTrades { get; set; }
        public int AcreFeetOfWaterPurchased { get; set; }
        public int AcreFeetOfWaterSold { get; set; }
    }
}