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
    public class HotelServiceTests
    {
        private Mock<IRepository<Hotel>> _mockHotelRepository;
        private HotelService _hotelService;

        [SetUp]
        public void Setup()
        {
            _mockHotelRepository = new Mock<IRepository<Hotel>>();
            _hotelService = new HotelService(_mockHotelRepository.Object);
        }

        // --- AddHotel Tests ---

        [Test]
        public void AddHotel_ValidHotel_AddsHotelToRepository()
        {
            var newHotel = new Hotel { Name = "Test Hotel", Description = "A nice place." };
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(new List<Hotel>());
            _mockHotelRepository.Setup(r => r.Add(It.IsAny<Hotel>()));

            _hotelService.AddHotel(newHotel);

            _mockHotelRepository.Verify(r => r.Add(It.Is<Hotel>(h => h.Name == "Test Hotel" && h.Id == 1)), Times.Once);
        }

        [Test]
        public void AddHotel_NullHotel_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _hotelService.AddHotel(null));
        }

        [Test]
        public void AddHotel_EmptyName_ThrowsValidationException()
        {
            var newHotel = new Hotel { Name = "", Description = "A nice place." };
            Assert.Throws<ValidationException>(() => _hotelService.AddHotel(newHotel));
        }

        [Test]
        public void AddHotel_HotelWithExistingId_ThrowsValidationException()
        {
            var existingHotels = new List<Hotel> { new Hotel { Id = 1, Name = "Existing Hotel" } };
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(existingHotels);
            var newHotel = new Hotel { Id = 1, Name = "New Hotel", Description = "New" };

            Assert.Throws<ValidationException>(() => _hotelService.AddHotel(newHotel));
        }

        // --- DeleteHotel Tests ---

        [Test]
        public void DeleteHotel_ExistingHotel_DeletesHotelFromRepository()
        {
            var hotelToDelete = new Hotel { Id = 1, Name = "Test Hotel" };
            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotelToDelete);
            _mockHotelRepository.Setup(r => r.Delete(1));

            _hotelService.DeleteHotel(1);

            _mockHotelRepository.Verify(r => r.Delete(1), Times.Once);
        }

        [Test]
        public void DeleteHotel_HotelNotFound_ThrowsValidationException()
        {
            _mockHotelRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Hotel)null);

            Assert.Throws<ValidationException>(() => _hotelService.DeleteHotel(999));
        }

        [Test]
        public void DeleteHotel_InvalidHotelId_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _hotelService.DeleteHotel(0));
        }

        // --- GetHotelDetails Tests ---

        [Test]
        public void GetHotelDetails_ExistingHotel_ReturnsHotel()
        {
            var hotel = new Hotel { Id = 1, Name = "Test Hotel" };
            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);

            var result = _hotelService.GetHotelDetails(1);

            Assert.That(result, Is.EqualTo(hotel));
        }

        [Test]
        public void GetHotelDetails_HotelNotFound_ReturnsNull()
        {
            _mockHotelRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Hotel)null);

            var result = _hotelService.GetHotelDetails(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetHotelDetails_InvalidHotelId_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _hotelService.GetHotelDetails(0));
        }

        // --- GetAllHotels Tests ---

        [Test]
        public void GetAllHotels_ReturnsAllHotels()
        {
            var hotels = new List<Hotel> { new Hotel { Id = 1 }, new Hotel { Id = 2 } };
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(hotels);

            var result = _hotelService.GetAllHotels();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(hotels));
        }

        // --- AddBookingApplication Tests ---

        [Test]
        public void AddBookingApplication_ValidApplication_AddsToHotelAndUpdatesRepository()
        {
            var hotel = new Hotel { Id = 1, Name = "Test Hotel", Applications = new List<BookingApplication>() };
            var newApplication = new BookingApplication { RoomType = "Standard", StartDate = DateTime.Today.AddDays(5), EndDate = DateTime.Today.AddDays(7), RequestText = "Quiet room" };

            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);
            _mockHotelRepository.Setup(r => r.Update(It.IsAny<Hotel>()));

            _hotelService.AddBookingApplication(1, newApplication);

            Assert.That(hotel.Applications.Count, Is.EqualTo(1));
            Assert.That(hotel.Applications[0].Id, Is.EqualTo(1));
            _mockHotelRepository.Verify(r => r.Update(hotel), Times.Once);
        }

        [Test]
        public void AddBookingApplication_HotelNotFound_ThrowsBookingException()
        {
            var newApplication = new BookingApplication { RoomType = "Standard", StartDate = DateTime.Today.AddDays(5), EndDate = DateTime.Today.AddDays(7) };
            _mockHotelRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Hotel)null);

            Assert.Throws<BookingException>(() => _hotelService.AddBookingApplication(999, newApplication));
        }

        [Test]
        public void AddBookingApplication_NullApplication_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _hotelService.AddBookingApplication(1, null));
        }

        [Test]
        public void AddBookingApplication_InvalidDates_ThrowsValidationException()
        {
            var hotel = new Hotel { Id = 1, Applications = new List<BookingApplication>() };
            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);

            var invalidApplication = new BookingApplication { RoomType = "Standard", StartDate = DateTime.Today.AddDays(7), EndDate = DateTime.Today.AddDays(5) }; // Start >= End
            Assert.Throws<ValidationException>(() => _hotelService.AddBookingApplication(1, invalidApplication));

            var pastApplication = new BookingApplication { RoomType = "Standard", StartDate = DateTime.Today.AddDays(-5), EndDate = DateTime.Today.AddDays(-3) }; // Start in past
            Assert.Throws<ValidationException>(() => _hotelService.AddBookingApplication(1, pastApplication));
        }

        // --- DeleteBookingApplication Tests ---

        [Test]
        public void DeleteBookingApplication_ExistingApplication_RemovesFromHotelAndUpdatesRepository()
        {
            var existingApplication = new BookingApplication { Id = 1, RoomType = "Standard" };
            var hotel = new Hotel { Id = 1, Name = "Test Hotel", Applications = new List<BookingApplication> { existingApplication } };

            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);
            _mockHotelRepository.Setup(r => r.Update(It.IsAny<Hotel>()));

            _hotelService.DeleteBookingApplication(1, 1);

            Assert.That(hotel.Applications.Count, Is.EqualTo(0));
            _mockHotelRepository.Verify(r => r.Update(hotel), Times.Once);
        }

        [Test]
        public void DeleteBookingApplication_ApplicationNotFound_ThrowsBookingException()
        {
            var hotel = new Hotel { Id = 1, Applications = new List<BookingApplication>() };
            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);

            Assert.Throws<BookingException>(() => _hotelService.DeleteBookingApplication(1, 999));
        }

        [Test]
        public void DeleteBookingApplication_HotelNotFound_ThrowsBookingException()
        {
            _mockHotelRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Hotel)null);

            Assert.Throws<BookingException>(() => _hotelService.DeleteBookingApplication(999, 1));
        }

        // --- UpdateBookingApplication Tests ---

        [Test]
        public void UpdateBookingApplication_ExistingApplication_UpdatesInHotelAndRepository()
        {
            var existingApplication = new BookingApplication { Id = 1, RoomType = "Standard", RequestText = "Old text" };
            var hotel = new Hotel { Id = 1, Name = "Test Hotel", Applications = new List<BookingApplication> { existingApplication } };
            var updatedApplication = new BookingApplication { Id = 1, RoomType = "Deluxe", RequestText = "New text", StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3) };

            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);
            _mockHotelRepository.Setup(r => r.Update(It.IsAny<Hotel>()));

            _hotelService.UpdateBookingApplication(1, updatedApplication);

            Assert.That(hotel.Applications.Count, Is.EqualTo(1));
            Assert.That(hotel.Applications[0].RequestText, Is.EqualTo("New text"));
            Assert.That(hotel.Applications[0].RoomType, Is.EqualTo("Deluxe"));
            _mockHotelRepository.Verify(r => r.Update(hotel), Times.Once);
        }

        [Test]
        public void UpdateBookingApplication_ApplicationNotFound_ThrowsBookingException()
        {
            var hotel = new Hotel { Id = 1, Applications = new List<BookingApplication>() };
            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);
            var updatedApplication = new BookingApplication { Id = 999, RoomType = "Standard", RequestText = "Text", StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(3) };

            Assert.Throws<BookingException>(() => _hotelService.UpdateBookingApplication(1, updatedApplication));
        }

        [Test]
        public void UpdateBookingApplication_NullApplication_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _hotelService.UpdateBookingApplication(1, null));
        }

        // --- GetBookingApplicationsForPeriod Tests ---

        [Test]
        public void GetBookingApplicationsForPeriod_HotelWithApplicationsInPeriod_ReturnsFilteredApplications()
        {
            var app1 = new BookingApplication { Id = 1, StartDate = new DateTime(2024, 7, 1), EndDate = new DateTime(2024, 7, 5) };
            var app2 = new BookingApplication { Id = 2, StartDate = new DateTime(2024, 7, 6), EndDate = new DateTime(2024, 7, 10) };
            var app3 = new BookingApplication { Id = 3, StartDate = new DateTime(2024, 8, 1), EndDate = new DateTime(2024, 8, 5) }; // Outside period

            var hotel = new Hotel { Id = 1, Applications = new List<BookingApplication> { app1, app2, app3 } };
            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);

            var result = _hotelService.GetBookingApplicationsForPeriod(1, new DateTime(2024, 7, 1), new DateTime(2024, 7, 15)).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(a => a.Id == 1), Is.True);
            Assert.That(result.Any(a => a.Id == 2), Is.True);
            Assert.That(result.Any(a => a.Id == 3), Is.False);
        }

        [Test]
        public void GetBookingApplicationsForPeriod_HotelNotFound_ThrowsBookingException()
        {
            _mockHotelRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Hotel)null);

            Assert.Throws<BookingException>(() => _hotelService.GetBookingApplicationsForPeriod(999, DateTime.Today, DateTime.Today.AddDays(1)));
        }

        [Test]
        public void GetBookingApplicationsForPeriod_InvalidDates_ThrowsValidationException()
        {
            var hotel = new Hotel { Id = 1, Applications = new List<BookingApplication>() };
            _mockHotelRepository.Setup(r => r.GetById(1)).Returns(hotel);

            Assert.Throws<ValidationException>(() => _hotelService.GetBookingApplicationsForPeriod(1, DateTime.Today.AddDays(5), DateTime.Today.AddDays(1))); // Start >= End
        }

        // --- SearchHotels Tests ---

        [Test]
        public void SearchHotels_WithMatchingKeyword_ReturnsFilteredHotels()
        {
            var hotels = new List<Hotel>
            {
                new Hotel { Id = 1, Name = "Grand Hotel", Description = "Luxurious" },
                new Hotel { Id = 2, Name = "City Inn", Description = "Budget friendly" },
                new Hotel { Id = 3, Name = "Grand Resort", Description = "Beachfront" }
            };
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(hotels);

            var result = _hotelService.SearchHotels("grand").ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(h => h.Id == 1), Is.True);
            Assert.That(result.Any(h => h.Id == 3), Is.True);
            Assert.That(result.Any(h => h.Id == 2), Is.False);
        }

        [Test]
        public void SearchHotels_NoMatchingKeyword_ReturnsEmptyList()
        {
            var hotels = new List<Hotel>
            {
                new Hotel { Id = 1, Name = "Grand Hotel", Description = "Luxurious" }
            };
            _mockHotelRepository.Setup(r => r.GetAll()).Returns(hotels);

            var result = _hotelService.SearchHotels("xyz").ToList();

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void SearchHotels_EmptyKeyword_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _hotelService.SearchHotels(""));
        }
    }
}