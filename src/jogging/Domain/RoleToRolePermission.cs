namespace Domain
{
    public class RoleToRolePermission
    {
        public int RoleId { get; set; }
        public UserRole Role { get; set; }
        public int PermissionId { get; set; }
        public RolePermission Permission { get; set; }
    }
}