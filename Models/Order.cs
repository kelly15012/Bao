using Bao.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bao.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        public string? Status { get; set; }

        [ForeignKey("BaoUser")]
        public string ? UserId { get; set; }

        // Navigation properties
        public virtual BaoUser ? User { get; set; }
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
}
