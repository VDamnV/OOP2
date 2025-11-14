using System;
using System.IO;
using HotelBookingSystem.BLL.Interfaces;
using HotelBookingSystem.BLL.Models;
using HotelBookingSystem.BLL.Services;
using HotelBookingSystem.DAL.Interfaces;
using HotelBookingSystem.DAL.Repositories;
using HotelBookingSystem.PL.Menus;

namespace HotelBookingSystem.PL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            string dataStoragePath = Path.Combine(AppContext.BaseDirectory, "DataStorage");
            string hotelsFilePath = Path.Combine(dataStoragePath, "hotels.json");
            string clientsFilePath = Path.Combine(dataStoragePath, "clients.json");
            string bookingsFilePath = Path.Combine(dataStoragePath, "bookings.json");

            IRepository<Hotel> hotelRepository = new FileRepository<Hotel>(hotelsFilePath);
            IRepository<Client> clientRepository = new FileRepository<Client>(clientsFilePath);
            IRepository<Booking> bookingRepository = new FileRepository<Booking>(bookingsFilePath);

            IHotelService hotelService = new HotelService(hotelRepository);
            IClientService clientService = new ClientService(clientRepository);
            IBookingService bookingService = new BookingService(bookingRepository, clientRepository, hotelRepository);
            SearchService searchService = new SearchService(hotelService, clientService);

            HotelMenu hotelMenu = new HotelMenu(hotelService);
            ClientMenu clientMenu = new ClientMenu(clientService);
            BookingMenu bookingMenu = new BookingMenu(bookingService, clientService, hotelService);
            MainMenu mainMenu = new MainMenu(hotelMenu, clientMenu, bookingMenu);

            mainMenu.Show();
        }
    }
}