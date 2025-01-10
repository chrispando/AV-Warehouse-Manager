namespace SounDesign_Web_02.Models
{
    public class SounDesignSearchProduct
    {
        public int id { get; set; }
        public string serial { get; set; }
        public string sku { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public ushort quantity { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public string site { get; set; }
        public string room { get; set; }
        public string technician { get; set; }
        public string signature { get; set; }

        public SounDesignSearchProduct() { }

        //Product
        public SounDesignSearchProduct(int id, string serial, string sku, string make, string model,
            ushort quantity, string description, string location)
        {
            this.id = id;
            this.serial = serial;
            this.sku = sku;
            this.make = make;
            this.model = model;
            this.quantity = quantity;
            this.description = description;
            this.location = location;
        }

        //Staging
        public SounDesignSearchProduct(int id, string serial, string sku, string make, string model,
            ushort quantity, string description, string site, string room)
        {
            this.id = id;
            this.serial = serial;
            this.sku = sku;
            this.make = make;
            this.model = model;
            this.quantity = quantity;
            this.description = description;
            this.site = site;
            this.room = room;
        }

        //Truck
        public SounDesignSearchProduct(int id, string serial, string sku, string make, string model,
            ushort quantity, string description, string site, string room,
            string technician, string signature)
        {
            this.id = id;
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
        }
    }
}
