using System.Collections.Generic;

namespace RailwayTicketApp.Models
{
    public class Wagon
    {
        public int WagonId { get; set; }
        public int TrainId { get; set; }
        public string WagonType { get; set; } // "Плацкарт", "Купе", "Люкс"
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }

        public virtual Train Train { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}