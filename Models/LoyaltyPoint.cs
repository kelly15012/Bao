using Bao.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bao.Models
{
    public class LoyaltyPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoyaltyPointId { get; set; }

        // Foreign Key
        [ForeignKey("BaoUser")]
        [Required]
        public string? UserId { get; set; }
        public virtual BaoUser? User { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public DateTime EarnedDate { get; set; }

        [StringLength(100)]
        [Required]
        public string ? Description { get; set; }
    }
}
