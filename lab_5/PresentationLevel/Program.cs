using lab_5.BusinessLogic;
using lab_5.BusinessLogic.DTOs;

namespace lab_5.PresentationLevel;

static class Program
{
    static void Main()
    {
        var entityService = EntityService.CreateJsonEntityService("Variant3_People.json");

        // === БЛОК ІНІЦІАЛІЗАЦІЇ ДАНИХ ===
        /*
        entityService.Clear();

        entityService.AddPerson(new StudentDTO
        {
            FirstName = "Maria",
            LastName = "Ivanenko",
            Gender = "Female",
            PassportId = 1111,
            StudentId = "KV1111",
            Year = 1,
            Residence = "Hostel 2.201" // Формат "номер.кімната"
        });

        entityService.AddPerson(new StudentDTO
        {
            FirstName = "Olena",
            LastName = "Petrenko",
            Gender = "Female",
            PassportId = 2222,
            StudentId = "KV2222",
            Year = 1,
            Residence = "Hostel 2.202"
        });

        entityService.AddPerson(new StudentDTO
        {
            FirstName = "Ivan",
            LastName = "Koval",
            Gender = "Male",
            PassportId = 3333,
            StudentId = "KV3333",
            Year = 1,
            Residence = "Hostel 1.101"
        });

        entityService.AddPerson(new StudentDTO
        {
            FirstName = "Anna",
            LastName = "Shevchenko",
            Gender = "Female",
            PassportId = 4444,
            StudentId = "KV4444",
            Year = 2,
            Residence = "Hostel 2.203"
        });

        entityService.AddPerson(new StudentDTO
        {
            FirstName = "Yulia",
            LastName = "Lysenko",
            Gender = "Female",
            PassportId = 5555,
            StudentId = "KV5555",
            Year = 1,
            Residence = "Kyiv, Main St. 5" // Не формат гуртожитку
        });

        entityService.AddPerson(new StudentDTO
        {
            FirstName = "Andriy",
            LastName = "Melnyk",
            Gender = "Male",
            PassportId = 6666,
            StudentId = "KV6666",
            Year = 3,
            Residence = "Needs Hostel" // Не формат "номер.кімната"
        });

        entityService.AddPerson(new StudentDTO
        {
            FirstName = "Kateryna",
            LastName = "Boiko",
            Gender = "Female",
            PassportId = 7777,
            StudentId = "KV7777",
            Year = 2,
            Residence = "" // Порожній рядок
        });

        entityService.AddPerson(new MusicianDTO
        {
            FirstName = "Mykola",
            LastName = "Leontovych",
            Gender = "Male",
            PassportId = 8888,
            Instrument = "Piano",
            SkillLevel = 50
        });

        entityService.AddPerson(new PilotDTO
        {
            FirstName = "Leonid",
            LastName = "Kadeniuk",
            Gender = "Male",
            PassportId = 9999,
            LicenseId = "UKR-001",
            FlightHours = 1000
        });
        entityService.SaveToFile();
        */
        // === КІНЕЦЬ БЛОКУ ІНІЦІАЛІЗАЦІЇ ===


        var result = entityService.LoadFromFile();
        if (!result.IsSuccess)
        {
            Console.WriteLine($"Error loading file: {result.Message}");
            return;
        }

        Console.WriteLine("=== Усі завантажені люди ===");
        Console.WriteLine(entityService.GetPeopleFullInfo());
        Console.WriteLine(new string('-', 40));

        Console.WriteLine("=== Студентки 1-го курсу в гуртожитку ===");
        Console.WriteLine(entityService.GetFirstYearFemaleHostelResidentsInfo());
        Console.WriteLine(new string('-', 40));

        Console.WriteLine("=== Поселення студентів... ===");
        Console.WriteLine(entityService.SettleStudentsInHostel());
        Console.WriteLine(new string('-', 40));

        Console.WriteLine("=== Музикант тренується ===");
        int musicianIndex = 8; 
        Console.WriteLine(entityService.PracticeMusic(musicianIndex));
        Console.WriteLine(entityService.PracticeMusic(musicianIndex));
        Console.WriteLine(new string('-', 40));
        
        Console.WriteLine("=== Катання на ковзанах (не потрібно для 'добре') ===");
        Console.WriteLine(entityService.GoSkating(0)); // [0] - Maria Ivanenko
        Console.WriteLine(new string('-', 40));

        Console.WriteLine("Збереження змін у файл...");
        var saveResult = entityService.SaveToFile();
        if (saveResult.IsSuccess)
        {
            Console.WriteLine("Зміни успішно збережено.");
        }
        else
        {
            Console.WriteLine($"Помилка збереження: {saveResult.Message}");
        }
    }
}