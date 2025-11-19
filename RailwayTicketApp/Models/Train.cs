using System.Collections.Generic;

namespace RailwayTicketApp.Models
{
    public class Train
    {
        public int TrainId { get; set; }
        public string TrainNumber { get; set; }
        public string TrainName { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public System.DateTime DepartureTime { get; set; }
        public System.DateTime ArrivalTime { get; set; }
        public int TotalSeats { get; set; }
        public decimal BasePrice { get; set; }

        public virtual ICollection<Wagon> Wagons { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}