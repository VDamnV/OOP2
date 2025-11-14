namespace HotelBookingSystem.BLL.Models
{
    public class Room
    {
        public int RoomNumber { get; set; }
        public string Type { get; set; }
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; } 
    }
}