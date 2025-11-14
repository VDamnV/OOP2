using System;

namespace HotelBookingSystem.BLL.Models
{
    public class BookingApplication
    {
        public int Id { get; set; }
        public string RoomType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RequestText { get; set; }
    }
}