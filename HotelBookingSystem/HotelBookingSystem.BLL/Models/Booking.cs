using System;

namespace HotelBookingSystem.BLL.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int HotelId { get; set; }
        public int RoomNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalCost { get; set; }

        public Client Client { get; set; }
        public Hotel Hotel { get; set; }
        public Room Room { get; set; }
    }
}