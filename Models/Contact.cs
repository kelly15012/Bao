using Bao.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bao.Models
{
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("BaoUser")]
        public string? UserId { get; set; }

        // Navigation properties
        public virtual BaoUser? User { get; set; }

        [Required]
        [StringLength(1000)]
        public required string Message { get; set; }
    }
}
