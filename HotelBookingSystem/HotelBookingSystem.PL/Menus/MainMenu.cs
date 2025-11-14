using System;

namespace HotelBookingSystem.PL.Menus
{
    public class MainMenu
    {
        private readonly HotelMenu _hotelMenu;
        private readonly ClientMenu _clientMenu;
        private readonly BookingMenu _bookingMenu;

        public MainMenu(HotelMenu hotelMenu, ClientMenu clientMenu, BookingMenu bookingMenu)
        {
            _hotelMenu = hotelMenu;
            _clientMenu = clientMenu;
            _bookingMenu = bookingMenu;
        }

        public void Show()
        {
            while (true)
            {
                Console.WriteLine("\n--- Головне меню системи бронювання готелів ---");
                Console.WriteLine("1. Управління готелями");
                Console.WriteLine("2. Управління клієнтами");
                Console.WriteLine("3. Управління замовленнями номерів");
                Console.WriteLine("0. Вийти");

                Console.Write("Виберіть опцію: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _hotelMenu.Show();
                        break;
                    case "2":
                        _clientMenu.Show();
                        break;
                    case "3":
                        _bookingMenu.Show();
                        break;
                    case "0":
                        Console.WriteLine("Дякуємо за використання системи!");
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Будь ласка, спробуйте ще.");
                        break;
                }
            }
        }
    }
}