using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models {

    public class Order
    {
        [Key]
        public int ID { get; set; }

        public bool Finalized { get; set; }

        public DateTime DateTime { get; set; }

        // Foreign key to Customer
        public int Customer_ID { get; set; }

        public Customer? Customer { get; set; }

        [StringLength(30)]
        public string? Customer_Name { get; set; }

        [StringLength(30)]
        public string? Customer_Address { get; set; }

        // Foreign key to Product
        public int Product_ID { get; set; }

        public Product? Product { get; set; }

        // Product information stored with the order
        [StringLength(25)]
        public string? Product_Name { get; set; }
    }
}