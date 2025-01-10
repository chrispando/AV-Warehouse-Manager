using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SounDesign_Web_02.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("Serial:")]

        public string? serial { get; set; }
        [DisplayName("SKU:")]

        public string? sku { get; set; }
        [DisplayName("Make:")]
        [Required]
        public string make { get; set; }
        [DisplayName("Model:")]
        [Required]
        public string model { get; set; }
        [DisplayName("Quantity:")]
        [Required]
        public ushort quantity { get; set; }
        [DisplayName("Description:")]
        [Required]
        public string description { get; set; }
        [DisplayName("Location:")]
        [Required]
        public string location { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdTimeStamp { get; set; }

        public string productUserStamp { get; set; }

        public Product() { }

        public Product(string serial, string sku, string make, string model,
            ushort quantity, string description, string location) {

            this.serial = serial;
            this.sku = sku;
            this.make = make;
            this.model = model;
            this.quantity = quantity;
            this.description = description;
            this.location = location;

        }

        public Product(string serial, string sku, string make, string model,
            ushort quantity, string description, string location,DateTime createdTimeStamp,string productUserStamp)
        {
            this.serial = serial;
            this.sku = sku;
            this.make = make;
            this.model = model;
            this.quantity = quantity;
            this.description = description;
            this.location = location;
            this.createdTimeStamp = createdTimeStamp;
            this.productUserStamp = productUserStamp;
        }

        public Product(int id, string serial, string sku, string make, string model,
            ushort quantity, string description, string location,DateTime createdTimeStamp, string productUserStamp)
        {
            this.ID = id;
            this.serial = serial;
            this.sku = sku;
            this.make = make;
            this.model = model;
            this.quantity = quantity;
            this.description = description;
            this.location = location;
            this.createdTimeStamp = createdTimeStamp;
            this.productUserStamp = productUserStamp;
        }

        public Staging ToStaging()
        {
            Staging s = new Staging();

            s.serial = serial;
            s.sku = sku;
            s.make = make;
            s.model = model;
            s.quantity = 0;
            s.description = description;
            s.site = "";
            s.room = "";
            s.createdTimeStamp = createdTimeStamp;

            return s;
        }
    }
}
