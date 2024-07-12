using Bao.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bao.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int categoryId { get; set; }

        [Required]
        [StringLength(50)]
        public required string categoryName { get; set; }

        [Required]
        public bool Status { get; set; }

        [Required]
        public DateTime CreateAt { get; set; }

        [Required]
        public DateTime ModifiedAt { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
