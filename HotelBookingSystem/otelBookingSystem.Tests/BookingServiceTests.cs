using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using HotelBookingSystem.BLL.Exceptions;
using HotelBookingSystem.BLL.Models;
using HotelBookingSystem.BLL.Services;
using HotelBookingSystem.DAL.Interfaces;

namespace HotelBookingSystem.Tests
{
    [TestFixture]
    public class BookingServiceTests
    {
        private Mock<IRepository<Booking>> _mockBookingRepository;
        private Mock<IRepository<Client>> _mockClientRepository;
        private Mock<IRepository<Hotel>> _mockHotelRepository;
        private BookingService _bookingService;

        [SetUp]
        public void Setup()
        {
            _mockBookingRepository = new Mock<IRepository<Booking>>();
            _mockClientRepository = new Mock<IRepository<Client>>();
            _mockHotelRepository = new Mock<IRepository<Hotel>>();
            _bookingService = new BookingService(_mockBookingRepository.Object, _mockClientRepository.Object, _mockHotelRepository.Object);
        }

        // --- CreateBooking Tests ---

        [Test]
        public void CreateBooking_ValidData_AddsBookingToRepository()
        {
            var client = new Client { Id = 1, FirstName = "Test", LastName = "Client", PhoneNumber = "123" };
            var hotel = new Hotel { Id = 1, Name = "Test Hotel", Description = "Desc", Rooms = new List<Room> { new Room { RoomNumber = 101, Type = "Standard", PricePerNight = 100, Capacity = 2 } } };
            
            _mockClientRepository.Setup(r => r.GetById(1)).Returns(client);
            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);
            _mockBookingRepository.Setup(r => r.GetAll()).Returns(new List<Booking>()); 
            _mockBookingRepository.Setup(r => r.Add(It.IsAny<Booking>()));

            _bookingService.CreateBooking(1, 1, 101, DateTime.Today.AddDays(1), DateTime.Today.AddDays(3));

            _mockBookingRepository.Verify(r => r.Add(It.IsAny<Booking>()), Times.Once);
        }

        [Test]
        public void CreateBooking_ClientNotFound_ThrowsBookingException()
        {
            _mockClientRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Client)null);
            _mockHotelRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns(new Hotel());

            Assert.Throws<BookingException>(() => _bookingService.CreateBooking(1, 1, 101, DateTime.Today.AddDays(1), DateTime.Today.AddDays(3)));
        }

        [Test]
        public void CreateBooking_HotelNotFound_ThrowsBookingException()
        {
            _mockClientRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns(new Client());
            _mockHotelRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Hotel)null);

            Assert.Throws<BookingException>(() => _bookingService.CreateBooking(1, 1, 101, DateTime.Today.AddDays(1), DateTime.Today.AddDays(3)));
        }

        [Test]
        public void CreateBooking_RoomNotFoundInHotel_ThrowsBookingException()
        {
            var client = new Client { Id = 1 };
            var hotel = new Hotel { Id = 1, Rooms = new List<Room>() }; // Empty rooms list
            
            _mockClientRepository.Setup(r => r.GetById(1)).Returns(client);
            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);

            Assert.Throws<BookingException>(() => _bookingService.CreateBooking(1, 1, 999, DateTime.Today.AddDays(1), DateTime.Today.AddDays(3)));
        }

        [Test]
        public void CreateBooking_RoomAlreadyBooked_ThrowsBookingException()
        {
            var client = new Client { Id = 1 };
            var hotel = new Hotel { Id = 1, Name = "Test Hotel", Description = "Desc", Rooms = new List<Room> { new Room { RoomNumber = 101, Type = "Standard", PricePerNight = 100, Capacity = 2 } } };
            var existingBooking = new Booking { Id = 1, HotelId = 1, RoomNumber = 101, CheckInDate = DateTime.Today.AddDays(1), CheckOutDate = DateTime.Today.AddDays(5) };

            _mockClientRepository.Setup(r => r.GetById(1)).Returns(client);
            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);
            _mockBookingRepository.Setup(r => r.GetAll()).Returns(new List<Booking> { existingBooking });

            Assert.Throws<BookingException>(() => _bookingService.CreateBooking(1, 1, 101, DateTime.Today.AddDays(2), DateTime.Today.AddDays(4)));
        }

        [Test]
        public void CreateBooking_InvalidDates_ThrowsValidationException()
        {
            var client = new Client { Id = 1 };
            var hotel = new Hotel { Id = 1, Rooms = new List<Room> { new Room { RoomNumber = 101, Type = "Standard", PricePerNight = 100, Capacity = 2 } } };

            _mockClientRepository.Setup(r => r.GetById(1)).Returns(client);
            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);
            _mockBookingRepository.Setup(r => r.GetAll()).Returns(new List<Booking>());

            Assert.Throws<ValidationException>(() => _bookingService.CreateBooking(1, 1, 101, DateTime.Today.AddDays(3), DateTime.Today.AddDays(1))); // CheckInDate >= CheckOutDate
            Assert.Throws<ValidationException>(() => _bookingService.CreateBooking(1, 1, 101, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1))); // CheckInDate in the past
        }

        // --- CancelBooking Tests ---

        [Test]
        public void CancelBooking_ExistingBooking_DeletesFromRepository()
        {
            var booking = new Booking { Id = 1, ClientId = 1, HotelId = 1, RoomNumber = 101, CheckInDate = DateTime.Today.AddDays(1), CheckOutDate = DateTime.Today.AddDays(3) };
            
            _mockBookingRepository.Setup(r => r.GetById(1)).Returns(booking);
            _mockBookingRepository.Setup(r => r.Delete(1));

            _bookingService.CancelBooking(1);

            _mockBookingRepository.Verify(r => r.Delete(1), Times.Once);
        }

        [Test]
        public void CancelBooking_BookingNotFound_ThrowsBookingException()
        {
            _mockBookingRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Booking)null);

            Assert.Throws<BookingException>(() => _bookingService.CancelBooking(999));
        }

        [Test]
        public void CancelBooking_InvalidBookingId_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _bookingService.CancelBooking(0));
        }

        // --- GetBookingDetails Tests ---

        [Test]
        public void GetBookingDetails_ExistingBooking_ReturnsBooking()
        {
            var booking = new Booking { Id = 1 };
            _mockBookingRepository.Setup(r => r.GetById(1)).Returns(booking);

            var result = _bookingService.GetBookingDetails(1);

            Assert.That(result, Is.EqualTo(booking));
        }

        [Test]
        public void GetBookingDetails_BookingNotFound_ReturnsNull()
        {
            _mockBookingRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Booking)null);

            var result = _bookingService.GetBookingDetails(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetBookingDetails_InvalidBookingId_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _bookingService.GetBookingDetails(0));
        }

        // --- GetAllBookings Tests ---

        [Test]
        public void GetAllBookings_ReturnsAllBookings()
        {
            var bookings = new List<Booking> { new Booking { Id = 1 }, new Booking { Id = 2 } };
            _mockBookingRepository.Setup(r => r.GetAll()).Returns(bookings);

            var result = _bookingService.GetAllBookings();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(bookings));
        }

        // --- GetBookedRooms Tests ---

        [Test]
        public void GetBookedRooms_HotelWithActiveBookings_ReturnsBookedRooms()
        {
            var hotel = new Hotel { Id = 1, Name = "Test Hotel", Rooms = new List<Room> { new Room { RoomNumber = 101 }, new Room { RoomNumber = 102 } } };
            var booking1 = new Booking { Id = 1, HotelId = 1, RoomNumber = 101, CheckOutDate = DateTime.Today.AddDays(5) };
            var booking2 = new Booking { Id = 2, HotelId = 1, RoomNumber = 102, CheckOutDate = DateTime.Today.AddDays(5) };
            var oldBooking = new Booking { Id = 3, HotelId = 1, RoomNumber = 101, CheckOutDate = DateTime.Today.AddDays(-1) }; // Expired

            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel> { hotel });
            _mockBookingRepository.Setup(r => r.GetAll()).Returns(new List<Booking> { booking1, booking2, oldBooking });

            var result = _bookingService.GetBookedRooms(1).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(r => r.RoomNumber == 101), Is.True);
            Assert.That(result.Any(r => r.RoomNumber == 102), Is.True);
        }

        [Test]
        public void GetBookedRooms_HotelNotFound_ThrowsBookingException()
        {
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel>());
            _mockBookingRepository.Setup(r => r.GetAll()).Returns(new List<Booking>());

            Assert.Throws<BookingException>(() => _bookingService.GetBookedRooms(999));
        }

        [Test]
        public void GetBookedRooms_InvalidHotelId_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _bookingService.GetBookedRooms(0));
        }

        // --- GetAvailableRooms Tests ---

        [Test]
        public void GetAvailableRooms_HotelWithAvailableRooms_ReturnsAvailableRooms()
        {
            var hotel = new Hotel { Id = 1, Name = "Test Hotel", Rooms = new List<Room> { new Room { RoomNumber = 101 }, new Room { RoomNumber = 102 }, new Room { RoomNumber = 103 } } };
            var booking = new Booking { Id = 1, HotelId = 1, RoomNumber = 101, CheckInDate = DateTime.Today.AddDays(1), CheckOutDate = DateTime.Today.AddDays(5) };

            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel> { hotel });
            _mockBookingRepository.Setup(r => r.GetAll()).Returns(new List<Booking> { booking });

            var result = _bookingService.GetAvailableRooms(1, DateTime.Today.AddDays(2), DateTime.Today.AddDays(4)).ToList();

            Assert.That(result.Count, Is.EqualTo(2)); // Rooms 102, 103 should be available
            Assert.That(result.Any(r => r.RoomNumber == 102), Is.True);
            Assert.That(result.Any(r => r.RoomNumber == 103), Is.True);
            Assert.That(result.Any(r => r.RoomNumber == 101), Is.False);
        }

        [Test]
        public void GetAvailableRooms_HotelNotFound_ThrowsBookingException()
        {
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel>());
            _mockBookingRepository.Setup(r => r.GetAll()).Returns(new List<Booking>());

            Assert.Throws<BookingException>(() => _bookingService.GetAvailableRooms(999, DateTime.Today, DateTime.Today.AddDays(1)));
        }

        [Test]
        public void GetAvailableRooms_InvalidDates_ThrowsValidationException()
        {
            var hotel = new Hotel { Id = 1, Rooms = new List<Room> { new Room { RoomNumber = 101 } } };
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel> { hotel });

            Assert.Throws<ValidationException>(() => _bookingService.GetAvailableRooms(1, DateTime.Today.AddDays(2), DateTime.Today.AddDays(1))); // CheckInDate >= CheckOutDate
        }

        // --- CalculateBookingCost Tests ---

        [Test]
        public void CalculateBookingCost_ValidData_ReturnsCorrectCost()
        {
            var hotel = new Hotel { Id = 1, Name = "Test Hotel", Rooms = new List<Room> { new Room { RoomNumber = 101, Type = "Standard", PricePerNight = 100 } } };
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel> { hotel });

            var cost = _bookingService.CalculateBookingCost(1, 101, DateTime.Today.AddDays(1), DateTime.Today.AddDays(4)); // 3 nights
            
            Assert.That(cost, Is.EqualTo(300.00m));
        }

        [Test]
        public void CalculateBookingCost_HotelNotFound_ThrowsBookingException()
        {
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel>());

            Assert.Throws<BookingException>(() => _bookingService.CalculateBookingCost(999, 101, DateTime.Today.AddDays(1), DateTime.Today.AddDays(2)));
        }

        [Test]
        public void CalculateBookingCost_RoomNotFound_ThrowsBookingException()
        {
            var hotel = new Hotel { Id = 1, Rooms = new List<Room>() };
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel> { hotel });

            Assert.Throws<BookingException>(() => _bookingService.CalculateBookingCost(1, 999, DateTime.Today.AddDays(1), DateTime.Today.AddDays(2)));
        }

        [Test]
        public void CalculateBookingCost_InvalidDates_ThrowsValidationException()
        {
            var hotel = new Hotel { Id = 1, Rooms = new List<Room> { new Room { RoomNumber = 101, PricePerNight = 100 } } };
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel> { hotel });

            Assert.Throws<ValidationException>(() => _bookingService.CalculateBookingCost(1, 101, DateTime.Today.AddDays(2), DateTime.Today.AddDays(1))); // CheckInDate >= CheckOutDate
            Assert.Throws<ValidationException>(() => _bookingService.CalculateBookingCost(1, 101, DateTime.Today.AddDays(1), DateTime.Today.AddDays(1))); // 0 nights
        }

        // --- GetClientsWithBookings Tests ---

        [Test]
        public void GetClientsWithBookings_HotelWithClients_ReturnsClients()
        {
            var hotel = new Hotel { Id = 1 };
            var client1 = new Client { Id = 1, FirstName = "A" };
            var client2 = new Client { Id = 2, FirstName = "B" };
            var booking1 = new Booking { Id = 1, HotelId = 1, ClientId = 1, CheckOutDate = DateTime.Today.AddDays(5) };
            var booking2 = new Booking { Id = 2, HotelId = 1, ClientId = 2, CheckOutDate = DateTime.Today.AddDays(5) };
            var oldBooking = new Booking { Id = 3, HotelId = 1, ClientId = 1, CheckOutDate = DateTime.Today.AddDays(-1) }; // Expired

            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel> { hotel });
            _mockClientRepository.Setup(r => r.GetAll()).Returns(new List<Client> { client1, client2 });
            _mockBookingRepository.Setup(r => r.GetAll()).Returns(new List<Booking> { booking1, booking2, oldBooking });

            var result = _bookingService.GetClientsWithBookings(1).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(c => c.Id == 1), Is.True);
            Assert.That(result.Any(c => c.Id == 2), Is.True);
        }

        [Test]
        public void GetClientsWithBookings_HotelNotFound_ThrowsBookingException()
        {
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel>());

            Assert.Throws<BookingException>(() => _bookingService.GetClientsWithBookings(999));
        }

        [Test]
        public void GetClientsWithBookings_InvalidHotelId_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _bookingService.GetClientsWithBookings(0));
        }
    }
}