using System.ComponentModel.DataAnnotations;


namespace BugBeading.Models {
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; } = string.Empty;

        [StringLength(25)]
        public string? ProductType { get; set; }

        public DateTime DateAdded { get; set; }

        [StringLength(100)]
        public string? Description { get; set; }

        [Range(0, 999999)]
        public double Price { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }
    }
}
