using System.ComponentModel.DataAnnotations;

namespace Bao.Models
{
    public class Contact
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }
    }
}
