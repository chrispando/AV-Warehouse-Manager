namespace SounDesign_Web_02.Models
{
    public class CalendarItem
    {
        public string title { get; set; }

        public DateTime start { get; set; }
        public string color { get; set; }
       

        public CalendarItem(string make, string model, DateTime time, string status)
        {
            this.title = make + " " + model;
            this.start = time;
            
        }
        public CalendarItem(Product p) {
            this.title = p.make + " " + p.model;
            this.start = p.createdTimeStamp;
            this.color = "blue";
           
        }

        public CalendarItem(Staging s)
        {
            this.title = s.make + " " + s.model;
            this.start = s.stagedTimeStamp;
            this.color = "black";

        }

        public CalendarItem(Truck t)
        {
            this.title = t.make + " " + t.model;
            this.start = t.truckTimeStamp;
            this.color = "green";

        }
    }
}
