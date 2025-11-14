using System;
using System.Linq;
using HotelBookingSystem.BLL.Exceptions;
using HotelBookingSystem.BLL.Interfaces;
using HotelBookingSystem.BLL.Models;
using HotelBookingSystem.PL.Utils;

namespace HotelBookingSystem.PL.Menus
{
    public class HotelMenu
    {
        private readonly IHotelService _hotelService;

        public HotelMenu(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public void Show()
        {
            while (true)
            {
                Console.WriteLine("\n--- Управління готелями ---");
                Console.WriteLine("1. Додати готель");
                Console.WriteLine("2. Видалити готель");
                Console.WriteLine("3. Переглянути деталі готелю");
                Console.WriteLine("4. Переглянути всі готелі");
                Console.WriteLine("5. Додати заявку на замовлення номера");
                Console.WriteLine("6. Видалити заявку на замовлення номера");
                Console.WriteLine("7. Замінити текст заявки на замовлення номера");
                Console.WriteLine("8. Переглянути заявки на замовлення номерів за термін");
                Console.WriteLine("9. Пошук готелів");
                Console.WriteLine("0. Назад до головного меню");

                Console.Write("Виберіть дію: ");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddHotel();
                            break;
                        case "2":
                            DeleteHotel();
                            break;
                        case "3":
                            GetHotelDetails();
                            break;
                        case "4":
                            GetAllHotels();
                            break;
                        case "5":
                            AddBookingApplication();
                            break;
                        case "6":
                            DeleteBookingApplication();
                            break;
                        case "7":
                            UpdateBookingApplication();
                            break;
                        case "8":
                            GetBookingApplicationsForPeriod();
                            break;
                        case "9":
                            SearchHotels();
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
                    Console.WriteLine($"Помилка готелю: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Виникла непередбачена помилка: {ex.Message}");
                }
            }
        }

        private void AddHotel()
        {
            Console.WriteLine("\n--- Додати готель ---");
            string name = ConsoleHelper.ReadString("Введіть назву готелю: ");
            string description = ConsoleHelper.ReadString("Введіть опис готелю: ");

            var newHotel = new Hotel { Name = name, Description = description };

            Console.Write("Скільки типів номерів ви хочете додати? ");
            int roomCount = ConsoleHelper.ReadInt("", 0);

            for (int i = 0; i < roomCount; i++)
            {
                Console.WriteLine($"\n--- Додавання номера {i + 1} ---");
                int roomNumber = ConsoleHelper.ReadInt("Введіть номер кімнати: ");
                string roomType = ConsoleHelper.ReadString("Введіть тип кімнати (напр., Standard, Deluxe, Suite): ");
                decimal pricePerNight = ConsoleHelper.ReadDecimal("Введіть ціну за ніч: ");
                int capacity = ConsoleHelper.ReadInt("Введіть місткість кімнати: ");
                newHotel.Rooms.Add(new Room { RoomNumber = roomNumber, Type = roomType, PricePerNight = pricePerNight, Capacity = capacity });
            }

            _hotelService.AddHotel(newHotel);
            Console.WriteLine("Готель успішно додано!");
        }

        private void DeleteHotel()
        {
            Console.WriteLine("\n--- Видалити готель ---");
            int hotelId = ConsoleHelper.ReadInt("Введіть ID готелю для видалення: ");

            _hotelService.DeleteHotel(hotelId);
            Console.WriteLine("Готель успішно видалено!");
        }

        private void GetHotelDetails()
        {
            Console.WriteLine("\n--- Деталі готелю ---");
            int hotelId = ConsoleHelper.ReadInt("Введіть ID готелю: ");

            var hotel = _hotelService.GetHotelDetails(hotelId);
            if (hotel != null)
            {
                Console.WriteLine($"ID: {hotel.Id}");
                Console.WriteLine($"Назва: {hotel.Name}");
                Console.WriteLine($"Опис: {hotel.Description}");
                Console.WriteLine("Номери:");
                if (hotel.Rooms.Any())
                {
                    foreach (var room in hotel.Rooms)
                    {
                        Console.WriteLine($"  Номер: {room.RoomNumber}, Тип: {room.Type}, Ціна за ніч: {room.PricePerNight:C}, Місткість: {room.Capacity}");
                    }
                }
                else
                {
                    Console.WriteLine("  Номери відсутні.");
                }
            }
            else
            {
                Console.WriteLine($"Готель з ID {hotelId} не знайдено.");
            }
        }

        private void GetAllHotels()
        {
            Console.WriteLine("\n--- Всі готелі ---");
            var hotels = _hotelService.GetAllHotels().ToList();
            if (hotels.Any())
            {
                foreach (var hotel in hotels)
                {
                    Console.WriteLine($"ID: {hotel.Id}, Назва: {hotel.Name}, Опис: {hotel.Description}");
                    Console.WriteLine($"  Кількість місць (загальна місткість номерів): {hotel.Rooms.Sum(r => r.Capacity)}");
                    Console.WriteLine("  Типи номерів:");
                    if (hotel.Rooms.Any())
                    {
                        foreach (var room in hotel.Rooms)
                        {
                            Console.WriteLine($"    Номер: {room.RoomNumber}, Тип: {room.Type}, Ціна: {room.PricePerNight:C}, Місткість: {room.Capacity}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("    Номери відсутні.");
                    }
                    Console.WriteLine("--------------------");
                }
            }
            else
            {
                Console.WriteLine("Готелі відсутні.");
            }
        }

        private void AddBookingApplication()
        {
            Console.WriteLine("\n--- Додати заявку на замовлення номера ---");
            int hotelId = ConsoleHelper.ReadInt("Введіть ID готелю: ");
            string roomType = ConsoleHelper.ReadString("Введіть бажаний тип кімнати: ");
            DateTime startDate = ConsoleHelper.ReadDateTime("Введіть бажану дату заїзду (РРРР-ММ-ДД): ");
            DateTime endDate = ConsoleHelper.ReadDateTime("Введіть бажану дату виїзду (РРРР-ММ-ДД): ");
            string requestText = ConsoleHelper.ReadString("Введіть текст заявки (побажання): ");

            var newApplication = new BookingApplication
            {
                RoomType = roomType,
                StartDate = startDate,
                EndDate = endDate,
                RequestText = requestText
            };

            _hotelService.AddBookingApplication(hotelId, newApplication);
            Console.WriteLine("Заявку на замовлення номера успішно додано!");
        }

        private void DeleteBookingApplication()
        {
            Console.WriteLine("\n--- Видалити заявку на замовлення номера ---");
            int hotelId = ConsoleHelper.ReadInt("Введіть ID готелю: ");
            int applicationId = ConsoleHelper.ReadInt("Введіть ID заявки для видалення: ");

            _hotelService.DeleteBookingApplication(hotelId, applicationId);
            Console.WriteLine("Заявку на замовлення номера успішно видалено!");
        }

        private void UpdateBookingApplication()
        {
            Console.WriteLine("\n--- Замінити текст заявки на замовлення номера ---");
            int hotelId = ConsoleHelper.ReadInt("Введіть ID готелю: ");
            int applicationId = ConsoleHelper.ReadInt("Введіть ID заявки для оновлення: ");

            var hotel = _hotelService.GetHotelDetails(hotelId);
            if (hotel == null)
            {
                Console.WriteLine($"Готель з ID {hotelId} не знайдено.");
                return;
            }

            var existingApplication = hotel.Applications.FirstOrDefault(a => a.Id == applicationId);
            if (existingApplication == null)
            {
                Console.WriteLine($"Заявку з ID {applicationId} не знайдено в готелі {hotel.Name}.");
                return;
            }

            Console.WriteLine($"Поточний текст заявки: {existingApplication.RequestText}");
            string newRequestText = ConsoleHelper.ReadString("Введіть новий текст заявки: ");

            existingApplication.RequestText = newRequestText;

            _hotelService.UpdateBookingApplication(hotelId, existingApplication);
            Console.WriteLine("Текст заявки успішно оновлено!");
        }

        private void GetBookingApplicationsForPeriod()
        {
            Console.WriteLine("\n--- Заявки на замовлення номерів за термін ---");
            int hotelId = ConsoleHelper.ReadInt("Введіть ID готелю: ");
            DateTime startDate = ConsoleHelper.ReadDateTime("Введіть початкову дату (РРРР-ММ-ДД): ");
            DateTime endDate = ConsoleHelper.ReadDateTime("Введіть кінцеву дату (РРРР-ММ-ДД): ");

            var applications = _hotelService.GetBookingApplicationsForPeriod(hotelId, startDate, endDate).ToList();
            if (applications.Any())
            {
                Console.WriteLine($"Заявки для готелю (ID: {hotelId}) з {startDate.ToShortDateString()} по {endDate.ToShortDateString()}:");
                foreach (var app in applications)
                {
                    Console.WriteLine($"  ID: {app.Id}, Тип кімнати: {app.RoomType}, Заїзд: {app.StartDate.ToShortDateString()}, Виїзд: {app.EndDate.ToShortDateString()}, Текст: {app.RequestText}");
                }
            }
            else
            {
                Console.WriteLine($"Заявки для готелю (ID: {hotelId}) за вказаний термін не знайдено або готель не знайдено.");
            }
        }

        private void SearchHotels()
        {
            Console.WriteLine("\n--- Пошук готелів ---");
            string keyword = ConsoleHelper.ReadString("Введіть ключове слово для пошуку: ");

            var hotels = _hotelService.SearchHotels(keyword).ToList();
            if (hotels.Any())
            {
                Console.WriteLine($"Знайдені готелі за ключовим словом '{keyword}':");
                foreach (var hotel in hotels)
                {
                    Console.WriteLine($"  ID: {hotel.Id}, Назва: {hotel.Name}, Опис: {hotel.Description}");
                }
            }
            else
            {
                Console.WriteLine($"Готелів за ключовим словом '{keyword}' не знайдено.");
            }
        }
    }
}