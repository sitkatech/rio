namespace Rio.Models.DataTransferObjects.Role
{
    public class RoleDto
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDisplayName { get; set; }
        public static string GetListRoute => "system-roles";

        public static string GetSingleRoute(int roleID)
        {
            return $"roles/{roleID}";
        }
    }
}
