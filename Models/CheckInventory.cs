namespace SounDesign_Web_02.Models
{
    public class CheckInventory
    {
        public string make { get; set; }
        public string model { get; set; }
        public int required { get; set; }
        public int stock { get; set; }
        public int order { get; set; }

        public CheckInventory(string make, string model, int required, int stock, int order)
        {
            this.make = make;
            this.model = model;
            this.required = required;
            this.stock = stock;
            this.order = order;

        }

    }
}
