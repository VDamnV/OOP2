using System;
using System.Linq;
using HotelBookingSystem.BLL.Exceptions;
using HotelBookingSystem.BLL.Interfaces;
using HotelBookingSystem.BLL.Models;
using HotelBookingSystem.PL.Utils;

namespace HotelBookingSystem.PL.Menus
{
    public class ClientMenu
    {
        private readonly IClientService _clientService;

        public ClientMenu(IClientService clientService)
        {
            _clientService = clientService;
        }

        public void Show()
        {
            while (true)
            {
                Console.WriteLine("\n--- Управління клієнтами ---");
                Console.WriteLine("1. Додати клієнта");
                Console.WriteLine("2. Видалити клієнта");
                Console.WriteLine("3. Оновити дані клієнта");
                Console.WriteLine("4. Переглянути дані конкретного клієнта");
                Console.WriteLine("5. Переглянути дані всіх клієнтів");
                Console.WriteLine("6. Відсортувати клієнтів за ім'ям");
                Console.WriteLine("7. Відсортувати клієнтів за прізвищем");
                Console.WriteLine("8. Пошук клієнтів");
                Console.WriteLine("0. Назад до головного меню");

                Console.Write("Виберіть дію: ");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddClient();
                            break;
                        case "2":
                            DeleteClient();
                            break;
                        case "3":
                            UpdateClient();
                            break;
                        case "4":
                            GetClientDetails();
                            break;
                        case "5":
                            GetAllClients();
                            break;
                        case "6":
                            GetClientsSortedByName();
                            break;
                        case "7":
                            GetClientsSortedByLastName();
                            break;
                        case "8":
                            SearchClients();
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Виникла непередбачена помилка: {ex.Message}");
                }
            }
        }

        private void AddClient()
        {
            Console.WriteLine("\n--- Додати клієнта ---");
            string firstName = ConsoleHelper.ReadString("Введіть ім'я клієнта: ");
            string lastName = ConsoleHelper.ReadString("Введіть прізвище клієнта: ");
            string phoneNumber = ConsoleHelper.ReadString("Введіть номер телефону клієнта: ");

            var newClient = new Client { FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber };
            _clientService.AddClient(newClient);
            Console.WriteLine("Клієнта успішно додано!");
        }

        private void DeleteClient()
        {
            Console.WriteLine("\n--- Видалити клієнта ---");
            int clientId = ConsoleHelper.ReadInt("Введіть ID клієнта для видалення: ");

            _clientService.DeleteClient(clientId);
            Console.WriteLine("Клієнта успішно видалено!");
        }

        private void UpdateClient()
        {
            Console.WriteLine("\n--- Оновити дані клієнта ---");
            int clientId = ConsoleHelper.ReadInt("Введіть ID клієнта для оновлення: ");
            var existingClient = _clientService.GetClientDetails(clientId);

            if (existingClient == null)
            {
                Console.WriteLine($"Клієнта з ID {clientId} не знайдено.");
                return;
            }

            Console.WriteLine($"Поточні дані: Ім'я: {existingClient.FirstName}, Прізвище: {existingClient.LastName}, Телефон: {existingClient.PhoneNumber}");
            string firstName = ConsoleHelper.ReadString($"Введіть нове ім'я клієнта (поточне: {existingClient.FirstName}): ", existingClient.FirstName);
            string lastName = ConsoleHelper.ReadString($"Введіть нове прізвище клієнта (поточне: {existingClient.LastName}): ", existingClient.LastName);
            string phoneNumber = ConsoleHelper.ReadString($"Введіть новий номер телефону клієнта (поточний: {existingClient.PhoneNumber}): ", existingClient.PhoneNumber);

            existingClient.FirstName = firstName;
            existingClient.LastName = lastName;
            existingClient.PhoneNumber = phoneNumber;

            _clientService.UpdateClient(existingClient);
            Console.WriteLine("Дані клієнта успішно оновлено!");
        }

        private void GetClientDetails()
        {
            Console.WriteLine("\n--- Деталі клієнта ---");
            int clientId = ConsoleHelper.ReadInt("Введіть ID клієнта: ");

            var client = _clientService.GetClientDetails(clientId);
            if (client != null)
            {
                Console.WriteLine($"ID: {client.Id}");
                Console.WriteLine($"Ім'я: {client.FirstName}");
                Console.WriteLine($"Прізвище: {client.LastName}");
                Console.WriteLine($"Телефон: {client.PhoneNumber}");
            }
            else
            {
                Console.WriteLine($"Клієнта з ID {clientId} не знайдено.");
            }
        }

        private void GetAllClients()
        {
            Console.WriteLine("\n--- Всі клієнти ---");
            var clients = _clientService.GetAllClients().ToList();
            if (clients.Any())
            {
                foreach (var client in clients)
                {
                    Console.WriteLine($"ID: {client.Id}, Ім'я: {client.FirstName}, Прізвище: {client.LastName}, Телефон: {client.PhoneNumber}");
                }
            }
            else
            {
                Console.WriteLine("Клієнти відсутні.");
            }
        }

        private void GetClientsSortedByName()
        {
            Console.WriteLine("\n--- Клієнти, відсортовані за ім'ям ---");
            var clients = _clientService.GetClientsSortedByName().ToList();
            if (clients.Any())
            {
                foreach (var client in clients)
                {
                    Console.WriteLine($"ID: {client.Id}, Ім'я: {client.FirstName}, Прізвище: {client.LastName}, Телефон: {client.PhoneNumber}");
                }
            }
            else
            {
                Console.WriteLine("Клієнти відсутні.");
            }
        }

        private void GetClientsSortedByLastName()
        {
            Console.WriteLine("\n--- Клієнти, відсортовані за прізвищем ---");
            var clients = _clientService.GetClientsSortedByLastName().ToList();
            if (clients.Any())
            {
                foreach (var client in clients)
                {
                    Console.WriteLine($"ID: {client.Id}, Ім'я: {client.FirstName}, Прізвище: {client.LastName}, Телефон: {client.PhoneNumber}");
                }
            }
            else
            {
                Console.WriteLine("Клієнти відсутні.");
            }
        }

        private void SearchClients()
        {
            Console.WriteLine("\n--- Пошук клієнтів ---");
            string keyword = ConsoleHelper.ReadString("Введіть ключове слово для пошуку: ");

            var clients = _clientService.SearchClients(keyword).ToList();
            if (clients.Any())
            {
                Console.WriteLine($"Знайдені клієнти за ключовим словом '{keyword}':");
                foreach (var client in clients)
                {
                    Console.WriteLine($"  ID: {client.Id}, Ім'я: {client.FirstName}, Прізвище: {client.LastName}, Телефон: {client.PhoneNumber}");
                }
            }
            else
            {
                Console.WriteLine($"Клієнтів за ключовим словом '{keyword}' не знайдено.");
            }
        }
    }
}