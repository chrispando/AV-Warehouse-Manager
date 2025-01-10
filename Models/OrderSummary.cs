namespace SounDesign_Web_02.Models
{
    public class OrderSummary
    {
        public string serial { get; set; }
        public string sku { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string description { get; set; }
        public string site { get; set; }
        public string room { get; set; }
        public DateTime createdTime { get; set; }
        public ushort inStock { get; set; }
        public ushort required { get; set; }
        public string action { get; set; }
        public string createdUserStamp { get; set; }

        public OrderSummary() { }
        public OrderSummary(string serial, string sku, string make,
            string model, string description, string site, string room, DateTime createdTime,
            ushort inStock, ushort required, string action,string createdUserStamp)
        {
            this.serial = serial;
            this.sku = sku;
            this.make = make;
            this.model = model;
            this.description = description;
            this.site = site;
            this.room = room;
            this.createdTime = createdTime;
            this.inStock = inStock;
            this.required = required;
            this.action = action;
            this.createdUserStamp = createdUserStamp;
        }
    }
}
