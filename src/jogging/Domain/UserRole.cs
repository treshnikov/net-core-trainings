using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class UserRole
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsSuperUser { get; set; }

        public ICollection<RoleToRolePermission> Permissions { get; set; }
    }
}