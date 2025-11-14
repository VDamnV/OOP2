using System;
using System.Collections.Generic;
using HotelBookingSystem.BLL.Models;

namespace HotelBookingSystem.BLL.Interfaces
{
    public interface IHotelService
    {
        void AddHotel(Hotel hotel);
        void DeleteHotel(int hotelId);
        Hotel GetHotelDetails(int hotelId);
        IEnumerable<Hotel> GetAllHotels();

        void AddBookingApplication(int hotelId, BookingApplication application);
        void DeleteBookingApplication(int hotelId, int applicationId);
        void UpdateBookingApplication(int hotelId, BookingApplication application);
        IEnumerable<BookingApplication> GetBookingApplicationsForPeriod(int hotelId, DateTime startDate, DateTime endDate);

        IEnumerable<Hotel> SearchHotels(string keyword);
    }
}