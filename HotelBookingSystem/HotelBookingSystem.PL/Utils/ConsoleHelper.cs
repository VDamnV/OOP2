using System;
using System.Globalization;

namespace HotelBookingSystem.PL.Utils
{
    public static class ConsoleHelper
    {
        public static string ReadString(string prompt, string defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    if (defaultValue != null)
                    {
                        return defaultValue;
                    }
                    Console.WriteLine("Введене значення не може бути порожнім. Будь ласка, спробуйте ще.");
                }
                else
                {
                    return input.Trim();
                }
            }
        }

        public static int ReadInt(string prompt, int? defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (int.TryParse(input, out int result) && result > 0)
                {
                    return result;
                }
                Console.WriteLine("Будь ласка, введіть дійсне додатне ціле число.");
            }
        }

        public static decimal ReadDecimal(string prompt, decimal? defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (decimal.TryParse(input, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal result) && result >= 0)
                {
                    return result;
                }
                Console.WriteLine("Будь ласка, введіть дійсне невід'ємне десяткове число.");
            }
        }

        public static DateTime ReadDateTime(string prompt, DateTime? defaultValue = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
                Console.WriteLine("Будь ласка, введіть дату у форматі РРРР-ММ-ДД.");
            }
        }
    }
}