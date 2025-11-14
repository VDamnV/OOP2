using System;
using System.Linq;
using HotelBookingSystem.BLL.Exceptions;
using HotelBookingSystem.BLL.Interfaces;
using HotelBookingSystem.PL.Utils;

namespace HotelBookingSystem.PL.Menus
{
    public class BookingMenu
    {
        private readonly IBookingService _bookingService;
        private readonly IClientService _clientService;
        private readonly IHotelService _hotelService;

        public BookingMenu(IBookingService bookingService, IClientService clientService, IHotelService hotelService)
        {
            _bookingService = bookingService;
            _clientService = clientService;
            _hotelService = hotelService;
        }

        public void Show()
        {
            while (true)
            {
                Console.WriteLine("\n--- Управління замовленнями номерів ---");
                Console.WriteLine("1. Забронювати номер");
                Console.WriteLine("2. Скасувати бронювання");
                Console.WriteLine("3. Переглянути деталі бронювання");
                Console.WriteLine("4. Переглянути всі бронювання");
                Console.WriteLine("5. Переглянути заброньовані номери в готелі");
                Console.WriteLine("6. Переглянути вільні номери в готелі за період");
                Console.WriteLine("7. Розрахувати вартість бронювання");
                Console.WriteLine("8. Переглянути клієнтів, які забронювали номери в готелі");
                Console.WriteLine("0. Назад до головного меню");

                Console.Write("Виберіть дію: ");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            CreateBooking();
                            break;
                        case "2":
                            CancelBooking();
                            break;
                        case "3":
                            GetBookingDetails();
                            break;
                        case "4":
                            GetAllBookings();
                            break;
                        case "5":
                            GetBookedRoomsInHotel();
                            break;
                        case "6":
                            GetAvailableRoomsInHotel();
                            break;
                        case "7":
                            CalculateBookingCost();
                            break;
                        case "8":
                            GetClientsWithBookingsInHotel();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Невірний вибір. Будь ласка, спробуйте ще.");
                            break;
                    }
                }
                catch (ValidationException ex)
                {
                    Console.WriteLine($"Помилка валідації: {ex.Message}");
                }
                catch (BookingException ex)
                {
                    Console.WriteLine($"Помилка бронювання: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Виникла непередбачена помилка: {ex.Message}");
                }
            }
        }

        private void CreateBooking()
        {
            Console.WriteLine("\n--- Забронювати номер ---");
            int clientId = ConsoleHelper.ReadInt("Введіть ID клієнта: ");
            int hotelId = ConsoleHelper.ReadInt("Введіть ID готелю: ");
            int roomNumber = ConsoleHelper.ReadInt("Введіть номер кімнати: ");
            DateTime checkInDate = ConsoleHelper.ReadDateTime("Введіть дату заїзду (РРРР-ММ-ДД): ");
            DateTime checkOutDate = ConsoleHelper.ReadDateTime("Введіть дату виїзду (РРРР-ММ-ДД): ");

            _bookingService.CreateBooking(clientId, hotelId, roomNumber, checkInDate, checkOutDate);
            Console.WriteLine("Номер успішно заброньовано!");
        }

        private void CancelBooking()
        {
            Console.WriteLine("\n--- Скасувати бронювання ---");
            int bookingId = ConsoleHelper.ReadInt("Введіть ID бронювання для скасування: ");

            _bookingService.CancelBooking(bookingId);
            Console.WriteLine("Бронювання успішно скасовано!");
        }

        private void GetBookingDetails()
        {
            Console.WriteLine("\n--- Деталі бронювання ---");
            int bookingId = ConsoleHelper.ReadInt("Введіть ID бронювання: ");

            var booking = _bookingService.GetBookingDetails(bookingId);
            if (booking != null)
            {
                Console.WriteLine($"ID бронювання: {booking.Id}");
                Console.WriteLine($"ID клієнта: {booking.ClientId}");
                Console.WriteLine($"ID готелю: {booking.HotelId}");
                Console.WriteLine($"Номер кімнати: {booking.RoomNumber}");
                Console.WriteLine($"Дата заїзду: {booking.CheckInDate.ToShortDateString()}");
                Console.WriteLine($"Дата виїзду: {booking.CheckOutDate.ToShortDateString()}");
                Console.WriteLine($"Загальна вартість: {booking.TotalCost:C}");
            }
            else
            {
                Console.WriteLine($"Бронювання з ID {bookingId} не знайдено.");
            }
        }

        private void GetAllBookings()
        {
            Console.WriteLine("\n--- Всі бронювання ---");
            var bookings = _bookingService.GetAllBookings().ToList();
            if (bookings.Any())
            {
                foreach (var booking in bookings)
                {
                    Console.WriteLine($"ID: {booking.Id}, Клієнт ID: {booking.ClientId}, Готель ID: {booking.HotelId}, Кімната: {booking.RoomNumber}, Заїзд: {booking.CheckInDate.ToShortDateString()}, Виїзд: {booking.CheckOutDate.ToShortDateString()}, Вартість: {booking.TotalCost:C}");
                }
            }
            else
            {
                Console.WriteLine("Бронювання відсутні.");
            }
        }

        private void GetBookedRoomsInHotel()
        {
            Console.WriteLine("\n--- Заброньовані номери в готелі ---");
            int hotelId = ConsoleHelper.ReadInt("Введіть ID готелю: ");

            var bookedRooms = _bookingService.GetBookedRooms(hotelId).ToList();
            if (bookedRooms.Any())
            {
                Console.WriteLine($"Заброньовані номери в готелі (ID: {hotelId}):");
                foreach (var room in bookedRooms)
                {
                    Console.WriteLine($"  Номер: {room.RoomNumber}, Тип: {room.Type}, Ціна за ніч: {room.PricePerNight:C}");
                }
            }
            else
            {
                Console.WriteLine($"В готелі (ID: {hotelId}) немає активних бронювань або готель не знайдено.");
            }
        }

        private void GetAvailableRoomsInHotel()
        {
            Console.WriteLine("\n--- Вільні номери в готелі за період ---");
            int hotelId = ConsoleHelper.ReadInt("Введіть ID готелю: ");
            DateTime checkInDate = ConsoleHelper.ReadDateTime("Введіть дату заїзду (РРРР-ММ-ДД): ");
            DateTime checkOutDate = ConsoleHelper.ReadDateTime("Введіть дату виїзду (РРРР-ММ-ДД): ");

            var availableRooms = _bookingService.GetAvailableRooms(hotelId, checkInDate, checkOutDate).ToList();
            if (availableRooms.Any())
            {
                Console.WriteLine($"Вільні номери в готелі (ID: {hotelId}) з {checkInDate.ToShortDateString()} по {checkOutDate.ToShortDateString()}:");
                foreach (var room in availableRooms)
                {
                    Console.WriteLine($"  Номер: {room.RoomNumber}, Тип: {room.Type}, Ціна за ніч: {room.PricePerNight:C}, Місткість: {room.Capacity}");
                }
            }
            else
            {
                Console.WriteLine($"Всі номери в готелі (ID: {hotelId}) зайняті на вказаний період, або готель не знайдено.");
            }
        }

        private void CalculateBookingCost()
        {
            Console.WriteLine("\n--- Розрахунок вартості бронювання ---");
            int hotelId = ConsoleHelper.ReadInt("Введіть ID готелю: ");
            int roomNumber = ConsoleHelper.ReadInt("Введіть номер кімнати: ");
            DateTime checkInDate = ConsoleHelper.ReadDateTime("Введіть дату заїзду (РРРР-ММ-ДД): ");
            DateTime checkOutDate = ConsoleHelper.ReadDateTime("Введіть дату виїзду (РРРР-ММ-ДД): ");

            decimal cost = _bookingService.CalculateBookingCost(hotelId, roomNumber, checkInDate, checkOutDate);
            Console.WriteLine($"Орієнтовна вартість бронювання: {cost:C}");
        }

        private void GetClientsWithBookingsInHotel()
        {
            Console.WriteLine("\n--- Клієнти, які забронювали номери в готелі ---");
            int hotelId = ConsoleHelper.ReadInt("Введіть ID готелю: ");

            var clients = _bookingService.GetClientsWithBookings(hotelId).ToList();
            if (clients.Any())
            {
                Console.WriteLine($"Клієнти, які забронювали номери в готелі (ID: {hotelId}):");
                foreach (var client in clients)
                {
                    Console.WriteLine($"  ID: {client.Id}, Ім'я: {client.FirstName} {client.LastName}, Телефон: {client.PhoneNumber}");
                }
            }
            else
            {
                Console.WriteLine($"Клієнтів, які забронювали номери в готелі (ID: {hotelId}), не знайдено або готель не знайдено.");
            }
        }
    }
}