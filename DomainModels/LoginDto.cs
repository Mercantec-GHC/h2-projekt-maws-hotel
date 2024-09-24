using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResult
    {
        public string? Token { get; set; }
        public int UserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}