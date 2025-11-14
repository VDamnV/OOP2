using System;
using System.Collections.Generic;
using System.Linq;
using HotelBookingSystem.BLL.Exceptions;
using HotelBookingSystem.BLL.Interfaces;
using HotelBookingSystem.BLL.Models;
using HotelBookingSystem.DAL.Interfaces;

namespace HotelBookingSystem.BLL.Services
{
    public class BookingService : IBookingService
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<Client> _clientRepository;
        private readonly IRepository<Hotel> _hotelRepository;

        public BookingService(IRepository<Booking> bookingRepository, IRepository<Client> clientRepository, IRepository<Hotel> hotelRepository)
        {
            _bookingRepository = bookingRepository;
            _clientRepository = clientRepository;
            _hotelRepository = hotelRepository;
        }

        public void CreateBooking(int clientId, int hotelId, int roomNumber, DateTime checkInDate, DateTime checkOutDate)
        {
            if (clientId <= 0 || hotelId <= 0 || roomNumber <= 0)
            {
                throw new ValidationException("Invalid input: Client ID, Hotel ID, and Room Number must be positive.");
            }
            if (checkInDate >= checkOutDate)
            {
                throw new ValidationException("Check-in date must be before check-out date.");
            }
            if (checkInDate < DateTime.Today)
            {
                throw new ValidationException("Check-in date cannot be in the past.");
            }

            var client = _clientRepository.GetById(clientId);
            if (client == null)
            {
                throw new BookingException($"Client with ID {clientId} not found.");
            }

            var hotel = _hotelRepository.GetById(hotelId);
            if (hotel == null)
            {
                throw new BookingException($"Hotel with ID {hotelId} not found.");
            }

            var room = hotel.Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (room == null)
            {
                throw new BookingException($"Room {roomNumber} not found in hotel {hotel.Name}.");
            }

            var existingBookings = _bookingRepository.GetAll().Where(b =>
                b.HotelId == hotelId &&
                b.RoomNumber == roomNumber &&
                ((checkInDate < b.CheckOutDate && checkOutDate > b.CheckInDate))
            ).ToList();

            if (existingBookings.Any())
            {
                throw new BookingException($"Room {roomNumber} in hotel {hotel.Name} is already booked for the specified period.");
            }

            decimal totalCost = CalculateBookingCost(hotelId, roomNumber, checkInDate, checkOutDate);

            var newBooking = new Booking
            {
                Id = _bookingRepository.GetAll().Any() ? _bookingRepository.GetAll().Max(b => b.Id) + 1 : 1,
                ClientId = clientId,
                HotelId = hotelId,
                RoomNumber = roomNumber,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                TotalCost = totalCost,
                Client = client, 
                Hotel = hotel,   
                Room = room      
            };

            _bookingRepository.Add(newBooking);
        }

        public void CancelBooking(int bookingId)
        {
            if (bookingId <= 0)
            {
                throw new ValidationException("Invalid input: Booking ID must be positive.");
            }

            var bookingToRemove = _bookingRepository.GetById(bookingId);
            if (bookingToRemove == null)
            {
                throw new BookingException($"Booking with ID {bookingId} not found.");
            }

            _bookingRepository.Delete(bookingId);
        }

        public Booking GetBookingDetails(int bookingId)
        {
            if (bookingId <= 0)
            {
                throw new ValidationException("Invalid input: Booking ID must be positive.");
            }
            return _bookingRepository.GetById(bookingId);
        }

        public IEnumerable<Booking> GetAllBookings()
        {
            return _bookingRepository.GetAll();
        }

        public IEnumerable<Room> GetBookedRooms(int hotelId)
        {
            if (hotelId <= 0)
            {
                throw new ValidationException("Invalid input: Hotel ID must be positive.");
            }

            var hotel = _hotelRepository.GetAll().FirstOrDefault(h => h.Id == hotelId);
            if (hotel == null)
            {
                throw new BookingException($"Hotel with ID {hotelId} not found.");
            }

            var bookedRoomNumbers = _bookingRepository.GetAll()
                .Where(b => b.HotelId == hotelId && b.CheckOutDate > DateTime.Now)
                .Select(b => b.RoomNumber)
                .Distinct()
                .ToList();

            return hotel.Rooms.Where(r => bookedRoomNumbers.Contains(r.RoomNumber)).ToList();
        }

        public IEnumerable<Room> GetAvailableRooms(int hotelId, DateTime checkInDate, DateTime checkOutDate)
        {
            if (hotelId <= 0)
            {
                throw new ValidationException("Invalid input: Hotel ID must be positive.");
            }
            if (checkInDate >= checkOutDate)
            {
                throw new ValidationException("Check-in date must be before check-out date.");
            }

            var hotel = _hotelRepository.GetAll().FirstOrDefault(h => h.Id == hotelId);
            if (hotel == null)
            {
                throw new BookingException($"Hotel with ID {hotelId} not found.");
            }

            var bookedRoomNumbers = _bookingRepository.GetAll()
                .Where(b =>
                    b.HotelId == hotelId &&
                    ((checkInDate < b.CheckOutDate && checkOutDate > b.CheckInDate))
                )
                .Select(b => b.RoomNumber)
                .Distinct()
                .ToList();

            return hotel.Rooms.Where(r => !bookedRoomNumbers.Contains(r.RoomNumber)).ToList();
        }

        public decimal CalculateBookingCost(int hotelId, int roomNumber, DateTime checkInDate, DateTime checkOutDate)
        {
            if (hotelId <= 0 || roomNumber <= 0)
            {
                throw new ValidationException("Invalid input: Hotel ID and Room Number must be positive.");
            }
            if (checkInDate >= checkOutDate)
            {
                throw new ValidationException("Check-in date must be before check-out date.");
            }

            var hotel = _hotelRepository.GetAll().FirstOrDefault(h => h.Id == hotelId);
            if (hotel == null)
            {
                throw new BookingException($"Hotel with ID {hotelId} not found.");
            }

            var room = hotel.Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (room == null)
            {
                throw new BookingException($"Room {roomNumber} not found in hotel {hotel.Name}.");
            }

            int numberOfNights = (int)(checkOutDate - checkInDate).TotalDays;
            if (numberOfNights <= 0)
            {
                throw new ValidationException("Number of nights must be positive.");
            }

            return room.PricePerNight * numberOfNights;
        }

        public IEnumerable<Client> GetClientsWithBookings(int hotelId)
        {
            if (hotelId <= 0)
            {
                throw new ValidationException("Invalid input: Hotel ID must be positive.");
            }

            var hotel = _hotelRepository.GetAll().FirstOrDefault(h => h.Id == hotelId);
            if (hotel == null)
            {
                throw new BookingException($"Hotel with ID {hotelId} not found.");
            }

            var clientIds = _bookingRepository.GetAll()
                .Where(b => b.HotelId == hotelId && b.CheckOutDate > DateTime.Now)
                .Select(b => b.ClientId)
                .Distinct()
                .ToList();

            var clients = _clientRepository.GetAll().Where(c => clientIds.Contains(c.Id)).ToList();

            return clients;
        }
    }
}