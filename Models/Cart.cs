using Bao.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bao.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }

        [ForeignKey("BaoUser")]
        [Required]
        public string ? UserId { get; set; }
        public virtual BaoUser ? User { get; set; }

        [ForeignKey("Product")]
        [Required]
        public int ProductId { get; set; }
        public virtual Product ? Product { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
