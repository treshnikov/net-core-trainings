using System.ComponentModel.DataAnnotations;
using Domain.Attributes;

namespace Domain
{
    public class User
    {
        public long Id { get; set; }
        
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Email { get; set; }

        [Required]
        public UserRole Role { get; set; }
        
        [Required]
        [NonFilterable]
        public string Password { get; set; }
        
        [Required]
        [NonFilterable]
        public string Salt { get; set; }
    }
}