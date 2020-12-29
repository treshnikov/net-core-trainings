using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class RolePermission
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        public ICollection<RoleToRolePermission> UserRoles { get; set; }
    }
}