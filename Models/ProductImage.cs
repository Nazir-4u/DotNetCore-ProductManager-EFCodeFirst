using System.ComponentModel.DataAnnotations.Schema;

namespace Product_Management.Models
{
    // Models/ProductImage.cs
    public class ProductImage
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }

}
