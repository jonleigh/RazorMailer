using System.ComponentModel.DataAnnotations;

namespace Sample.Core.Email.Models
{
    public class WelcomeModel
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; } 
    }
}