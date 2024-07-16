using System.ComponentModel.DataAnnotations;

namespace Bao.Models
{
    public class Contact
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public required string UserName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(1000)]
        public required string Message { get; set; }
    }
}
