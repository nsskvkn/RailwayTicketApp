using System;

namespace RailwayTicketApp.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int TrainId { get; set; }
        public int WagonId { get; set; }
        public string PassengerName { get; set; }
        public string PassengerDocument { get; set; }
        public int SeatNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } // "Заброньовано", "Продано", "Скасовано"

        public virtual Train Train { get; set; }
        public virtual Wagon Wagon { get; set; }
    }
}