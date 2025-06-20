using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Product_Management.Models
{

    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<ProductImage> Images { get; set; }
    }


}
