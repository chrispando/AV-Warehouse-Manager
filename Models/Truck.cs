using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SounDesign_Web_02.Models
{
    public class Truck
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
        [DisplayName("Technician")]
        public string technician { get; set; }
        [DisplayName("Signature")]
        public string signature { get; set; }

        public DateTime createdTimeStamp { get; set; }
        public DateTime stagedTimeStamp { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime truckTimeStamp { get; set; }

        public string productUserStamp { get; set; }
        public string stagingUserStamp { get; set; }
        public string truckUserStamp{get;set;}

        public Truck() { }
        public Truck(int id, string serial, string sku, string make, string model,
            ushort quantity, string description, string site, string room,
            string technician, string signature, DateTime createdTimeStamp, DateTime stagedTimeStamp, 
            string productUserStamp, string stagingUserStamp)
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
            this.technician = technician;
            this.signature = signature;
            this.createdTimeStamp = createdTimeStamp;
            this.stagedTimeStamp = stagedTimeStamp;
            this.productUserStamp = productUserStamp;
            this.stagingUserStamp = stagingUserStamp;
        }
        public Truck(string serial, string sku, string make, string model,
           ushort quantity, string description, string site, string room,
           string technician, string signature,DateTime createdTimeStamp,DateTime stagedTimeStamp,
           DateTime truckTimeStamp,
           string productUserStamp,string stagingUserStamp,string truckUserStamp)
        {
            this.serial = serial;
            this.sku = sku;
            this.make = make;
            this.model = model;
            this.quantity = quantity;
            this.description = description;
            this.site = site;
            this.room = room;
            this.technician = technician;
            this.signature = signature;
            this.createdTimeStamp = createdTimeStamp;
            this.stagedTimeStamp = stagedTimeStamp;
            this.truckTimeStamp = truckTimeStamp;
            this.productUserStamp = productUserStamp;
            this.stagingUserStamp = stagingUserStamp;
            this.truckUserStamp = truckUserStamp;
        }

        public Staging ToStaging()
        {
            return new Staging(this.serial, this.sku,
                this.make, this.model, this.quantity, this.description, this.site, 
                this.room,this.createdTimeStamp,this.productUserStamp);
        }
    }
}
