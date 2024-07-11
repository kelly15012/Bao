using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bao.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        public required string ProductName { get; set; }

        [StringLength(1000)]
        public string ? ProductDescription { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        public byte[] ? CoverImage { get; set; }

        public string ? FileName { get; set; }

        public string ? ContentType { get; set; }

        // Foreign Key
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        //virtual keyword is used to allow lazy loading
        public virtual Category? Category { get; set; }

        [Required]
        public bool Status { get; set; }

        [Required]
        public DateTime CreateAt { get; set; }

        [Required]
        public DateTime ModifiedAt { get; set; }

    }
}
