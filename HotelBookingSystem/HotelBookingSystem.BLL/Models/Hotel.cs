using System.Collections.Generic;

namespace HotelBookingSystem.BLL.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<BookingApplication> Applications { get; set; } = new List<BookingApplication>();
    }
}