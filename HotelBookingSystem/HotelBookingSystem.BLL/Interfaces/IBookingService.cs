using System;
using System.Collections.Generic;
using HotelBookingSystem.BLL.Models;

namespace HotelBookingSystem.BLL.Interfaces
{
    public interface IBookingService
    {
        void CreateBooking(int clientId, int hotelId, int roomNumber, DateTime checkInDate, DateTime checkOutDate);
        void CancelBooking(int bookingId);
        Booking GetBookingDetails(int bookingId);
        IEnumerable<Booking> GetAllBookings();
        IEnumerable<Room> GetBookedRooms(int hotelId);
        IEnumerable<Room> GetAvailableRooms(int hotelId, DateTime checkInDate, DateTime checkOutDate);
        decimal CalculateBookingCost(int hotelId, int roomNumber, DateTime checkInDate, DateTime checkOutDate);
        IEnumerable<Client> GetClientsWithBookings(int hotelId);
    }
}