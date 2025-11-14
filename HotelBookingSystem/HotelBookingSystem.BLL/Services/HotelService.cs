using System;
using System.Collections.Generic;
using System.Linq;
using HotelBookingSystem.BLL.Exceptions;
using HotelBookingSystem.BLL.Interfaces;
using HotelBookingSystem.BLL.Models;
using HotelBookingSystem.DAL.Interfaces;

namespace HotelBookingSystem.BLL.Services
{
    public class HotelService : IHotelService
    {
        private readonly IRepository<Hotel> _hotelRepository;

        public HotelService(IRepository<Hotel> hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public void AddHotel(Hotel hotel)
        {
            if (hotel == null)
            {
                throw new ValidationException("Hotel cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(hotel.Name))
            {
                throw new ValidationException("Hotel name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(hotel.Description))
            {
                throw new ValidationException("Hotel description cannot be empty.");
            }

            if (_hotelRepository.GetAll().Any(h => h.Id == hotel.Id))
            {
                throw new ValidationException($"Hotel with ID {hotel.Id} already exists.");
            }

            hotel.Id = _hotelRepository.GetAll().Any() ? _hotelRepository.GetAll().Max(h => h.Id) + 1 : 1;
            _hotelRepository.Add(hotel);
        }

        public void DeleteHotel(int hotelId)
        {
            if (hotelId <= 0)
            {
                throw new ValidationException("Invalid input: Hotel ID must be positive.");
            }

            var hotelToRemove = _hotelRepository.GetById(hotelId);
            if (hotelToRemove == null)
            {
                throw new ValidationException($"Hotel with ID {hotelId} not found.");
            }

            _hotelRepository.Delete(hotelId);
        }

        public Hotel GetHotelDetails(int hotelId)
        {
            if (hotelId <= 0)
            {
                throw new ValidationException("Invalid input: Hotel ID must be positive.");
            }
            return _hotelRepository.GetById(hotelId);
        }

        public IEnumerable<Hotel> GetAllHotels()
        {
            return _hotelRepository.GetAll();
        }

        public void AddBookingApplication(int hotelId, BookingApplication application)
        {
            if (hotelId <= 0)
            {
                throw new ValidationException("Invalid input: Hotel ID must be positive.");
            }
            if (application == null)
            {
                throw new ValidationException("Booking application cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(application.RoomType))
            {
                throw new ValidationException("Room type for booking application cannot be empty.");
            }
            if (application.StartDate >= application.EndDate)
            {
                throw new ValidationException("Application start date must be before end date.");
            }
            if (application.StartDate < DateTime.Today)
            {
                throw new ValidationException("Application start date cannot be in the past.");
            }

            var hotel = _hotelRepository.GetById(hotelId);
            if (hotel == null)
            {
                throw new BookingException($"Hotel with ID {hotelId} not found.");
            }

            application.Id = hotel.Applications.Any() ? hotel.Applications.Max(a => a.Id) + 1 : 1;
            hotel.Applications.Add(application);
            _hotelRepository.Update(hotel);
        }

        public void DeleteBookingApplication(int hotelId, int applicationId)
        {
            if (hotelId <= 0 || applicationId <= 0)
            {
                throw new ValidationException("Invalid input: Hotel ID and Application ID must be positive.");
            }

            var hotel = _hotelRepository.GetById(hotelId);
            if (hotel == null)
            {
                throw new BookingException($"Hotel with ID {hotelId} not found.");
            }

            var applicationToRemove = hotel.Applications.FirstOrDefault(a => a.Id == applicationId);
            if (applicationToRemove == null)
            {
                throw new BookingException($"Booking application with ID {applicationId} not found in hotel {hotel.Name}.");
            }

            hotel.Applications.Remove(applicationToRemove);
            _hotelRepository.Update(hotel);
        }

        public void UpdateBookingApplication(int hotelId, BookingApplication application)
        {
            if (hotelId <= 0)
            {
                throw new ValidationException("Invalid input: Hotel ID must be positive.");
            }
            if (application == null)
            {
                throw new ValidationException("Booking application cannot be null.");
            }
            if (application.Id <= 0)
            {
                throw new ValidationException("Invalid input: Application ID must be positive for update.");
            }
            if (string.IsNullOrWhiteSpace(application.RoomType))
            {
                throw new ValidationException("Room type for booking application cannot be empty.");
            }
            if (application.StartDate >= application.EndDate)
            {
                throw new ValidationException("Application start date must be before end date.");
            }
            if (application.StartDate < DateTime.Today)
            {
                throw new ValidationException("Application start date cannot be in the past.");
            }

            var hotel = _hotelRepository.GetById(hotelId);
            if (hotel == null)
            {
                throw new BookingException($"Hotel with ID {hotelId} not found.");
            }

            var existingApplicationIndex = hotel.Applications.FindIndex(a => a.Id == application.Id);
            if (existingApplicationIndex == -1)
            {
                throw new BookingException($"Booking application with ID {application.Id} not found in hotel {hotel.Name} for update.");
            }

            hotel.Applications[existingApplicationIndex] = application;
            _hotelRepository.Update(hotel);
        }

        public IEnumerable<BookingApplication> GetBookingApplicationsForPeriod(int hotelId, DateTime startDate, DateTime endDate)
        {
            if (hotelId <= 0)
            {
                throw new ValidationException("Invalid input: Hotel ID must be positive.");
            }
            if (startDate >= endDate)
            {
                throw new ValidationException("Start date must be before end date.");
            }

            var hotel = _hotelRepository.GetById(hotelId);
            if (hotel == null)
            {
                throw new BookingException($"Hotel with ID {hotelId} not found.");
            }

            return hotel.Applications.Where(app =>
                (app.StartDate < endDate && app.EndDate > startDate)
            ).ToList();
        }

        public IEnumerable<Hotel> SearchHotels(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ValidationException("Search keyword cannot be empty.");
            }

            string lowerKeyword = keyword.ToLower();
            return _hotelRepository.GetAll()
                .Where(h => h.Name.ToLower().Contains(lowerKeyword) ||
                            h.Description.ToLower().Contains(lowerKeyword))
                .ToList();
        }
    }
}