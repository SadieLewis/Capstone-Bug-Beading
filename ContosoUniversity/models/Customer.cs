using System.ComponentModel.DataAnnotations;

namespace BugBeading.Models {
    public class Customer
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; } = string.Empty;

        public int PhoneNumber { get; set; }

        [StringLength(30)]
        public string? Address { get; set; }

    }
}