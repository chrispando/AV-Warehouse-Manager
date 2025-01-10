using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SounDesign_Web_02.Models
{
    public class Staging
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Serial")]
        public string? serial { get; set; }
        [DisplayName("SKU")]
        public string? sku { get; set; }
        [DisplayName("Make")]
        public string make { get; set; }
        [DisplayName("Model")]
        public string model { get; set; }
        [DisplayName("Quantity")]
        public ushort quantity { get; set; }
        [DisplayName("Description")]
        public string description { get; set; }
        [DisplayName("Site")]
        public string site { get; set; }
        [DisplayName("Room")]
        public string room { get; set; }


        public DateTime createdTimeStamp { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime stagedTimeStamp { get; set; }

        public string productUserStamp { get; set; }
        public string stagingUserStamp { get; set; }

        public Staging() { }
        public Staging(int id, string serial, string sku, string make, string model,
            ushort quantity, string description, string site, string room, DateTime createdTimeStamp, 
            string productUserStamp)
        {
            this.Id = id;
            this.serial = serial;
            this.sku = sku;
            this.make = make;
            this.model = model;
            this.quantity = quantity;
            this.description = description;
            this.site = site;
            this.room = room;
            this.createdTimeStamp = createdTimeStamp;
            this.productUserStamp = productUserStamp;
        }

        public Staging(string serial, string sku, string make, string model,
            ushort quantity, string description, string site, string room,DateTime createdTimeStamp,
            string productUserStamp)
        {

            this.serial = serial;
            this.sku = sku;
            this.make = make;
            this.model = model;
            this.quantity = quantity;
            this.description = description;
            this.site = site;
            this.room = room;
            this.createdTimeStamp = createdTimeStamp;
            this.productUserStamp= productUserStamp;
        }

        public Staging(string serial, string sku, string make, string model,
            ushort quantity, string description, string site, string room, DateTime createdTimeStamp,
            DateTime stagedTimeStamp,
            string productUserStamp,string stagedUserStamp)
        {

            this.serial = serial;
            this.sku = sku;
            this.make = make;
            this.model = model;
            this.quantity = quantity;
            this.description = description;
            this.site = site;
            this.room = room;
            this.createdTimeStamp = createdTimeStamp;
            this.stagedTimeStamp = stagedTimeStamp;
            this.productUserStamp = productUserStamp;
            this.stagingUserStamp = stagedUserStamp;
        }
        public Staging(Product p)
        {
            this.serial = p.serial;
            this.sku = p.sku;
            this.make = p.make;
            this.model = p.model;
            this.quantity = p.quantity;
            this.description = p.description;
            this.site = "";
            this.room = "";
            this.createdTimeStamp = p.createdTimeStamp;
            this.productUserStamp = p.productUserStamp;
        }

        public Staging(OrderSummary os)
        {
            this.serial = os.serial;
            this.sku = os.sku;
            this.make = os.make;
            this.model = os.model;
            this.quantity = os.required;
            this.description = os.description;
            this.site = os.site;
            this.room = os.room;
            this.createdTimeStamp = os.createdTime;
            this.productUserStamp = os.createdUserStamp;
        }
    }
}
