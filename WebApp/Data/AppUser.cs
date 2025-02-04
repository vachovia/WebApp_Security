using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Data
{
    public class AppUser : IdentityUser
    {
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Department { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Position { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
       
    }
}
